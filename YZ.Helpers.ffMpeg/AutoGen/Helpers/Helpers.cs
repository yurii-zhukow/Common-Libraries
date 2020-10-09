using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using FFmpeg.AutoGen;


namespace SDK.FFMpeg {

    public sealed unsafe class H264VideoStreamEncoder : IDisposable {
        private readonly Size _frameSize;
        private readonly int _linesizeU;
        private readonly int _linesizeV;
        private readonly int _linesizeY;
        private readonly AVCodec* _pCodec;
        private readonly AVCodecContext* _pCodecContext;
        private readonly Stream _stream;
        private readonly int _uSize;
        private readonly int _ySize;

        static H264VideoStreamEncoder() {
#pragma warning disable 618
            ffmpeg.avcodec_register_all();
#pragma warning restore 618
        }

        public H264VideoStreamEncoder(Stream stream, int fps, Size frameSize) {
            _stream = stream;
            _frameSize = frameSize;

            var codecId = AVCodecID.AV_CODEC_ID_H264;
            _pCodec = ffmpeg.avcodec_find_encoder(codecId);
            if (_pCodec == null) throw new InvalidOperationException("Codec not found.");

            _pCodecContext = ffmpeg.avcodec_alloc_context3(_pCodec);
            _pCodecContext->width = frameSize.Width;
            _pCodecContext->height = frameSize.Height;
            _pCodecContext->time_base = new AVRational { num = 1, den = fps };
            _pCodecContext->pix_fmt = AVPixelFormat.AV_PIX_FMT_YUV420P;
            ffmpeg.av_opt_set(_pCodecContext->priv_data, "preset", "veryslow", 0);

            ffmpeg.avcodec_open2(_pCodecContext, _pCodec, null).ThrowExceptionIfError();

            _linesizeY = frameSize.Width;
            _linesizeU = frameSize.Width / 2;
            _linesizeV = frameSize.Width / 2;

            _ySize = _linesizeY * frameSize.Height;
            _uSize = _linesizeU * frameSize.Height / 2;
        }

        public void Dispose() {
            ffmpeg.avcodec_close(_pCodecContext);
            ffmpeg.av_free(_pCodecContext);
            ffmpeg.av_free(_pCodec);
        }

        public void Encode(AVFrame frame) {
            if (frame.format != (int)_pCodecContext->pix_fmt) throw new ArgumentException("Invalid pixel format.", nameof(frame));
            if (frame.width != _frameSize.Width) throw new ArgumentException("Invalid width.", nameof(frame));
            if (frame.height != _frameSize.Height) throw new ArgumentException("Invalid height.", nameof(frame));
            if (frame.linesize[0] != _linesizeY) throw new ArgumentException("Invalid Y linesize.", nameof(frame));
            if (frame.linesize[1] != _linesizeU) throw new ArgumentException("Invalid U linesize.", nameof(frame));
            if (frame.linesize[2] != _linesizeV) throw new ArgumentException("Invalid V linesize.", nameof(frame));
            if (frame.data[1] - frame.data[0] != _ySize) throw new ArgumentException("Invalid Y data size.", nameof(frame));
            if (frame.data[2] - frame.data[1] != _uSize) throw new ArgumentException("Invalid U data size.", nameof(frame));

            var pPacket = ffmpeg.av_packet_alloc();
            try {
                int error;
                do {
                    ffmpeg.avcodec_send_frame(_pCodecContext, &frame).ThrowExceptionIfError();

                    error = ffmpeg.avcodec_receive_packet(_pCodecContext, pPacket);
                } while (error == ffmpeg.AVERROR(ffmpeg.EAGAIN));

                error.ThrowExceptionIfError();

                using (var packetStream = new UnmanagedMemoryStream(pPacket->data, pPacket->size)) packetStream.CopyTo(_stream);
            } finally {
                ffmpeg.av_packet_unref(pPacket);
            }
        }
    }

    internal static class FFmpegHelper {

        public static unsafe string av_strerror(int error) {
            var bufferSize = 1024;
            var buffer = stackalloc byte[bufferSize];
            ffmpeg.av_strerror(error, buffer, (ulong)bufferSize);
            var message = Marshal.PtrToStringAnsi((IntPtr)buffer);
            return message;
        }

        public static int ThrowExceptionIfError(this int error) {
            if (error < 0) throw new ApplicationException(av_strerror(error));
            return error;
        }

    }

    public sealed unsafe class VideoConverter : IDisposable {

        private readonly SwsContext* _pConvertContext;

        public VideoConverter(Size sourceSize,
            AVPixelFormat sourcePixelFormat,
            Size destinationSize,
            AVPixelFormat destinationPixelFormat) {
            _pConvertContext = ffmpeg.sws_getContext(
                sourceSize.Width,
                sourceSize.Height,
                sourcePixelFormat,
                destinationSize.Width,
                destinationSize.Height,
                destinationPixelFormat,
                ffmpeg.SWS_FAST_BILINEAR,
                null,
                null,
                null
            );
            if (_pConvertContext == null) throw new ApplicationException("Could not initialize the conversion context.");

            var convertedFrameBufferSize = ffmpeg.av_image_get_buffer_size(destinationPixelFormat, destinationSize.Width, destinationSize.Height, 1);
            var convertedFrameBufferPtr = Marshal.AllocHGlobal(convertedFrameBufferSize);
            var dstData = new byte_ptrArray4();
            var dstLinesize = new int_array4();

            ffmpeg.av_image_fill_arrays(ref dstData, ref dstLinesize, (byte*)convertedFrameBufferPtr, destinationPixelFormat, destinationSize.Width, destinationSize.Height, 1);
        }

        public void Dispose() { }

        public AVFrame Convert(AVFrame sourceFrame) {
            var dstData = new byte_ptrArray4();
            var dstLinesize = new int_array4();

            ffmpeg.sws_scale(_pConvertContext, sourceFrame.data, sourceFrame.linesize, 0, sourceFrame.height, dstData, dstLinesize);

            return new AVFrame();
        }

    }
    public sealed unsafe class VideoFrameConverter : IDisposable {
        private readonly IntPtr _convertedFrameBufferPtr;
        private readonly Size _destinationSize;
        private readonly byte_ptrArray4 _dstData;
        private readonly int_array4 _dstLinesize;
        private readonly SwsContext* _pConvertContext;

        public VideoFrameConverter(Size sourceSize, AVPixelFormat sourcePixelFormat,
            Size destinationSize, AVPixelFormat destinationPixelFormat) {
            _destinationSize = destinationSize;

            _pConvertContext = ffmpeg.sws_getContext(sourceSize.Width, sourceSize.Height, sourcePixelFormat,
            destinationSize.Width,
            destinationSize.Height, destinationPixelFormat,
            ffmpeg.SWS_FAST_BILINEAR, null, null, null);
            if (_pConvertContext == null) throw new ApplicationException("Could not initialize the conversion context.");

            var convertedFrameBufferSize = ffmpeg.av_image_get_buffer_size(destinationPixelFormat, destinationSize.Width, destinationSize.Height, 1);
            _convertedFrameBufferPtr = Marshal.AllocHGlobal(convertedFrameBufferSize);
            _dstData = new byte_ptrArray4();
            _dstLinesize = new int_array4();

            ffmpeg.av_image_fill_arrays(ref _dstData, ref _dstLinesize, (byte*)_convertedFrameBufferPtr, destinationPixelFormat, destinationSize.Width, destinationSize.Height, 1);
        }

        public void Dispose() {
            Marshal.FreeHGlobal(_convertedFrameBufferPtr);
            ffmpeg.sws_freeContext(_pConvertContext);
        }

        public AVFrame Convert(AVFrame sourceFrame) {
            ffmpeg.sws_scale(_pConvertContext, sourceFrame.data, sourceFrame.linesize, 0, sourceFrame.height, _dstData, _dstLinesize);

            var data = new byte_ptrArray8();
            data.UpdateFrom(_dstData);
            var linesize = new int_array8();
            linesize.UpdateFrom(_dstLinesize);

            return new AVFrame {
                data = data,
                linesize = linesize,
                width = _destinationSize.Width,
                height = _destinationSize.Height
            };
        }
    }


    public sealed unsafe class VideoStreamDecoder : IDisposable {
        private readonly AVCodecContext* pCodecContext;
        private readonly AVFormatContext* pFormatContext;
        private readonly int streamIndex;
        private readonly AVFrame* pFrame;
        private readonly AVPacket* pPacket;


        public VideoStreamDecoder(string url) {

            this.pFormatContext = ffmpeg.avformat_alloc_context();
            var pFormatContext = this.pFormatContext;
            pFormatContext->audio_codec = null;
            pFormatContext->video_codec_id = AVCodecID.AV_CODEC_ID_H264;
            pFormatContext->duration = 5 * ffmpeg.AV_TIME_BASE; // 5 sec

            
            ffmpeg.avformat_open_input(&pFormatContext, url, null, null).ThrowExceptionIfError();

            ffmpeg.avformat_find_stream_info(this.pFormatContext, null).ThrowExceptionIfError();

            // find the first video stream
            AVStream* pStream = null;
            for (var i = 0; i < this.pFormatContext->nb_streams; i++)
                if (this.pFormatContext->streams[i]->codec->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO) {
                    pStream = this.pFormatContext->streams[i];
                    break;
                }

            if (pStream == null) throw new InvalidOperationException("Could not find video stream.");

            streamIndex = pStream->index;
            pCodecContext = pStream->codec;

            var codecId = pCodecContext->codec_id;
            var pCodec = ffmpeg.avcodec_find_decoder(codecId);
            if (pCodec == null) throw new InvalidOperationException("Unsupported codec.");

            ffmpeg.avcodec_open2(pCodecContext, pCodec, null).ThrowExceptionIfError();

            CodecName = ffmpeg.avcodec_get_name(codecId);
            FrameSize = new Size(pCodecContext->width, pCodecContext->height);
            PixelFormat = pCodecContext->pix_fmt;

            pPacket = ffmpeg.av_packet_alloc();
            pFrame = ffmpeg.av_frame_alloc();
        }

        public string CodecName { get; }
        public Size FrameSize { get; }
        public AVPixelFormat PixelFormat { get; }

        public void Dispose() {
            ffmpeg.av_frame_unref(pFrame);
            ffmpeg.av_free(pFrame);

            ffmpeg.av_packet_unref(pPacket);
            ffmpeg.av_free(pPacket);

            ffmpeg.avcodec_close(pCodecContext);
            var pFormatContext = this.pFormatContext;
            ffmpeg.avformat_close_input(&pFormatContext);
        }

        public bool TryDecodeNextFrame(out AVFrame frame) {
            ffmpeg.av_frame_unref(pFrame);
            int error;
            do {
                try {
                    do {
                        error = ffmpeg.av_read_frame(pFormatContext, pPacket);
                        if (error == ffmpeg.AVERROR_EOF) {
                            frame = *pFrame;
                            return false;
                        }

                        error.ThrowExceptionIfError();
                    } while (pPacket->stream_index != streamIndex);

                    ffmpeg.avcodec_send_packet(pCodecContext, pPacket).ThrowExceptionIfError();
                } finally {
                    ffmpeg.av_packet_unref(pPacket);
                }

                error = ffmpeg.avcodec_receive_frame(pCodecContext, pFrame);
            } while (error == ffmpeg.AVERROR(ffmpeg.EAGAIN));

            error.ThrowExceptionIfError();
            frame = *pFrame;
            return true;
        }

        public IReadOnlyDictionary<string, string> GetContextInfo() {
            AVDictionaryEntry* tag = null;
            var result = new Dictionary<string, string>();
            while ((tag = ffmpeg.av_dict_get(pFormatContext->metadata, "", tag, ffmpeg.AV_DICT_IGNORE_SUFFIX)) != null) {
                var key = Marshal.PtrToStringAnsi((IntPtr)tag->key);
                var value = Marshal.PtrToStringAnsi((IntPtr)tag->value);
                result.Add(key, value);
            }

            return result;
        }
    }
}
