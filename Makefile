runtime:
	@dotnet publish --no-self-contained -c Release

win10-x64:
	@dotnet publish -c Release -r win10-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeAllContentForSelfExtract=true -p:PublishTrimmed=True -p:TrimMode=CopyUsed

linux-x64:
	@dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeAllContentForSelfExtract=true -p:PublishTrimmed=True -p:TrimMode=CopyUsed

linux-arm:
	@dotnet publish -c Release -r linux-arm --self-contained true -p:PublishSingleFile=true -p:IncludeAllContentForSelfExtract=true -p:PublishTrimmed=True -p:TrimMode=CopyUsed

all:
	@make runtime
	@make win10-x64
	@make linux-x68
	@make linux-arm
