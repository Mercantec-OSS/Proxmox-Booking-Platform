apt update
apt install -y wget
wget https://github.com/PowerShell/PowerShell/releases/download/v7.4.1/powershell_7.4.1-1.deb_amd64.deb
dpkg -i powershell_7.4.1-1.deb_amd64.deb
apt install -f
rm powershell_7.4.1-1.deb_amd64.deb
