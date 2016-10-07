# DevOps eszközök alapja: kódból infrastruktúrát 

## Szinopszis
     
A számítási felhővel megérkezett az önkiszolgáló IT forradalma, és a DevOps egyre forróbb témává válik. Felhőtől, konténerektől és automatizációtól hangos a világ.
A komplex problémák komplex környezeteket kívánnak. A DevOps eszközök, módszertanok és gyakorlatok segítségével, az infrastruktúra kódként történő leírásával megismételhetővé,
verziókezeltté és automatizálhatóvá válik az informatikai környezetek létrehozása. Ebben az előadásban megismerhetitek, hogyan hozhattok létre akár komplex topológiákat
az Azure erőforrássablonok és keresztplatformos parancssori eszközök segítségével, hogyan automatizálhatjátok Linux és Windows virtuális gépek konfiguráció-menedzsmentjét.

## Előadás demo anyagok

* [Üres sablon](../blob/master/Session1-Introduction_to_DevOps-Infrastructure_as_Code/empty.json)
* Meglévő erőforrások sablonjának exportálása Azure portálon
* [Azure quickstart galéria](https://github.com/Azure/azure-quickstart-templates)
* [armviz.io](http://armviz.io/)

Demo környezet:
![demo environment](../blob/master/Session1-Introduction_to_DevOps-Infrastructure_as_Code/customscript.png)

## Eszközök és hasznos anyagok

* [Visual Studio Code](https://code.visualstudio.com/)
* [Azure command-line interface (x-plat)](https://azure.microsoft.com/en-us/downloads/)
* [Node.js](https://nodejs.org/en/)
* [Chocolatey](https://chocolatey.org/)

## Windows 10 Anniversary - Bash (Linux Subsystem)

Engedélyezd a **Developer mode**-ot az operációs rendszereden! Ezt vagy a beállításokban, vagy az alábbi PowerShell parancsokkal teheted meg:

```powershell
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock" /t REG_DWORD /f /v "AllowAllTrustedApps" /d "1"
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock" /t REG_DWORD /f /v "AllowDevelopmentWithoutDevLicense" /d "1"
``` 

Bash telepítése parancssorból:

```powershell
Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Windows-Subsystem-Linux
lxrun /install
```

## Azure Resource Manager Tools telepítése Visual Studio Code-ban

Nyisd meg a *Quick Open* (**CTRL+P**) eszköztárt, illeszt be az alábbi parancsot, és nyomj entert.
`ext install azurerm-vscode-tools`

## Azure CLI telepítése NPM-el

```bash
npm install -g azure-cli
```

## JQ telepítése 

Windows:

```powershell
chocolatey install jq
```

Ubuntu:

```bash
sudo apt-get install jq
```