$ErrorActionPreference = 'Stop';

msbuild -m ScreenMask.sln /p:Configuration=Release /p:Platform='Any CPU'
if (-not $?) { exit 1 }
