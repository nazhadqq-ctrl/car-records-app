Add-Type -AssemblyName System.Drawing
$imgPath = 'C:\Users\Nazha\.gemini\antigravity-ide\brain\e49d7456-6fc0-48f6-837d-8c0cf4372fb6\car_lab_app_icon_1788086548325.jpg'
$img = [System.Drawing.Image]::FromFile($imgPath)

# Save PNGs
$img.Save('public\app-icon.png', [System.Drawing.Imaging.ImageFormat]::Png)
if (Test-Path 'dist-package\public') { $img.Save('dist-package\public\app-icon.png', [System.Drawing.Imaging.ImageFormat]::Png) }
if (Test-Path 'android-system\www') { $img.Save('android-system\www\app-icon.png', [System.Drawing.Imaging.ImageFormat]::Png) }

function Generate-Ico($srcImg, $outFile) {
    $sizes = @(256, 128, 64, 48, 32, 16)
    $streams = @()
    foreach ($s in $sizes) {
        $bmp = New-Object System.Drawing.Bitmap($s, $s)
        $g = [System.Drawing.Graphics]::FromImage($bmp)
        $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
        $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
        $g.DrawImage($srcImg, 0, 0, $s, $s)
        $g.Dispose()
        $ms = New-Object System.IO.MemoryStream
        $bmp.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
        $bmp.Dispose()
        $streams += @{ Size = $s; Stream = $ms }
    }

    $fs = [System.IO.File]::Create($outFile)
    $bw = New-Object System.IO.BinaryWriter($fs)

    $bw.Write([UInt16]0)
    $bw.Write([UInt16]1)
    $bw.Write([UInt16]$sizes.Count)

    $offset = 6 + (16 * $sizes.Count)
    foreach ($item in $streams) {
        $sz = $item.Size
        $w = if ($sz -ge 256) { 0 } else { [byte]$sz }
        $h = if ($sz -ge 256) { 0 } else { [byte]$sz }
        $bw.Write([byte]$w)
        $bw.Write([byte]$h)
        $bw.Write([byte]0)
        $bw.Write([byte]0)
        $bw.Write([UInt16]1)
        $bw.Write([UInt16]32)
        $bw.Write([UInt32]$item.Stream.Length)
        $bw.Write([UInt32]$offset)
        $offset += $item.Stream.Length
    }

    foreach ($item in $streams) {
        $bytes = $item.Stream.ToArray()
        $bw.Write($bytes)
        $item.Stream.Dispose()
    }

    $bw.Flush()
    $fs.Close()
}

Generate-Ico $img 'app.ico'
Generate-Ico $img 'public\favicon.ico'
if (Test-Path 'dist-package') {
    Generate-Ico $img 'dist-package\app.ico'
    if (Test-Path 'dist-package\public') {
        Generate-Ico $img 'dist-package\public\favicon.ico'
    }
}
$img.Dispose()
Write-Host 'ICONS GENERATED SUCCESSFULLY!'
