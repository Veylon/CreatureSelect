@echo off
copy /y CreatureSelect.dll "F:\Steam\steamapps\common\Pathfinder Kingmaker\Mods\CreatureSelect"
copy /y info.json "F:\Steam\steamapps\common\Pathfinder Kingmaker\Mods\CreatureSelect"
cd /d "F:\Steam\steamapps\common\Pathfinder Kingmaker\Mods\CreatureSelect"
del *.cache
REM pause