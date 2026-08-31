
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $src = 'C:\Users\Nazha\Downloads\car-management-system\dist-package'
    $dst = 'C:\Users\Nazha\Downloads\car-management-system\Car-Management-Tablet-Setup.zip'
    if (Test-Path $dst) { Remove-Item $dst -Force }
    [System.IO.Compression.ZipFile]::CreateFromDirectory($src, $dst, [System.IO.Compression.CompressionLevel]::Optimal, $false)
  