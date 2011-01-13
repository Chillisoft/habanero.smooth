if exist temp goto dostuff
md temp
:dostuff
cd temp
svn co http://delicious:8080/svn/habanero/HabaneroCommunity/BuildScripts/habanero/%1 habanero --username chilli --password chilli --quiet --no-auth-cache
cd habanero
rmdir /s /q .svn
rake --rakefile habanero-library-rakefile.rb
cd..
cd..