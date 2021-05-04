Set-WinUILanguageOverride -Language en-US
$name = $args[0]
print "Bump version for $path ..."
#Get Path to csproj
$project = "$name\$name.csproj"
$path = "$PSScriptRoot\$project"

print $path

$xml = [xml](Get-Content $path)


$version = $xml.Project.PropertyGroup.Version

$major, $minor, $build  = $version.Split(".")

$build = [Convert]::ToInt32($build,10)+1

$xml.Project.PropertyGroup.Version = "$major.$minor.$build"

#Save csproj (XML)
$xml.Save($path)
Set-WinUILanguageOverride 