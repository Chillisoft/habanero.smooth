svn co http://delicious:8080/svn/habanero/HabaneroCommunity/BuildScripts/habanero/%1 temp/habanero --username chilli --password chilli --quiet --no-auth-cache
cd temp/habanero
rmdir /s /q .svn
rake --rakefile habanero-library-rakefile.rb
cd..
cd..