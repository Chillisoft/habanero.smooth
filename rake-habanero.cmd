if exist temp goto dostuff
md temp
:dostuff
cd temp
svn co http://delicious:8080/svn/habanero/HabaneroCommunity/BuildScripts/habanero/%1 habanero
cd habanero
rmdir /s /q .svn
rake --rakefile habanero-library-rakefile.rb
cd..
cd..