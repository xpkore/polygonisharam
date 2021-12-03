
# git clone https://github.com/xpkore/polygonisharam.git
# sh ~/polygonisharam/haram/init.sh www.google.com

cd ~/polygonisharam
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y apt-transport-https
sudo apt-get update
sudo apt-get install -y dotnet-sdk-3.1
cd ~/polygonisharam
dotnet build
cd ~/
echo "dotnet ~/polygonisharam/bin/Debug/netcoreapp3.1/haram.dll $1" > ~/run.sh
sh run.sh
