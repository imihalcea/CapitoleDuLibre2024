
set -ex
dotnet publish -c Release -r linux-arm64 --self-contained
rsync -avz --delete ./bin/Release/net8.0/linux-arm64/publish/ rpi4:./spike_Oxyplot_Avalonia
