@echo off
rem Сохраняем текущий каталог
set "current_dir=%CD%"

rem Изменяем текущий каталог
pushd D:\C_Sharp_Projects\GreatSportEventWeb\GreatSportEventWeb

rem Выполняем необходимые действия
D:\C_Sharp_Projects\GreatSportEventWeb\GreatSportEventWeb\bin\Release\net7.0\GreatSportEventWeb.exe

rem Возвращаемся к предыдущему каталогу
popd

rem Восстанавливаем предыдущий текущий каталог
cd /d "%current_dir%"