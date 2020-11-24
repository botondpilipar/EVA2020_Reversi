$FileExtensions = '*.ade', '*.adp', '*.apk', '*.appx', '*.appxbundle', '*.bat', '*.cab', '*.chm', '*.cmd', '*.com', '*.cpl', '*.dll', '*.dmg', '*.exe', '*.hta', '*.ins', '*.isp', '*.iso', '*.jar', '*.js', '*.jse', '*.lib', '*.lnk', '*.mde', '*.msc', '*.msi', '*.msix', '*.msixbundle', '*.msp', '*.mst', '*.nsh', '*.pif', '*.ps1', '*.scr', '*.sct', '*.shb', '*.sys', '*.vb', '*.vbe', '*.vbs', '*.vxd', '*.wsc', '*.wsf', '*.wsh', '*.pdb', '*.cache';

$Self='RemoveBinaries.ps1'

Remove-Item * -Recurse -Include ([string]::Join(" ", $FileExtensions)) -Exclude $Self