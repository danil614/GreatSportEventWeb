@echo off
rem ��������� ������� �������
set "current_dir=%CD%"

rem �������� ������� �������
pushd D:\C_Sharp_Projects\GreatSportEventWeb\GreatSportEventWeb

rem ��������� ����������� ��������
D:\C_Sharp_Projects\GreatSportEventWeb\GreatSportEventWeb\bin\Release\net7.0\GreatSportEventWeb.exe

rem ������������ � ����������� ��������
popd

rem ��������������� ���������� ������� �������
cd /d "%current_dir%"