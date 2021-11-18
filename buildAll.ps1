$allPlatforms = @(
	'win10-arm64',
	'win10-arm'
	'win10-x64',

	'linux-arm64',
	'linux-arm',
	'linux-x64'
);

foreach ($platform in $allPlatforms) {
	dotnet publish -c Release -r $platform --self-contained true -p:PublishSingleFile=true -p:IncludeAllContentForSelfExtract=true -p:PublishTrimmed=True -p:TrimMode=CopyUsed
}
dotnet publish --no-self-contained -c Release