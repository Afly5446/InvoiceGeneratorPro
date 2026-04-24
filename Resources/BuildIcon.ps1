# Генерирует AppIcon.ico (System.Drawing: GetHicon + Icon.Save(Stream), без ImageFormat.Icon).
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

$dir = $PSScriptRoot
$out = Join-Path $dir 'AppIcon.ico'
if (Test-Path $out) { Remove-Item $out -Force }

$size = 64
$bmp = $null
try {
    $bmp = New-Object System.Drawing.Bitmap ($size), ($size)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    try {
        $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
        $g.Clear([System.Drawing.Color]::FromArgb(255, 79, 70, 229))

        $fontSize = [single]([int]($size * 0.42))
        $font = $null
        $style = [System.Drawing.FontStyle]::Bold
        $unit = [System.Drawing.GraphicsUnit]::Pixel
        foreach ($name in @('Segoe UI', 'Arial')) {
            try {
                $font = New-Object System.Drawing.Font -ArgumentList @($name, $fontSize, $style, $unit)
                break
            }
            catch { }
        }
        if ($null -eq $font) {
            $ff = [System.Drawing.FontFamily]::GenericSansSerif
            $font = New-Object System.Drawing.Font -ArgumentList @($ff, $fontSize, $style, $unit)
        }
        try {
            $sf = New-Object System.Drawing.StringFormat
            try {
                $sf.Alignment = [System.Drawing.StringAlignment]::Center
                $sf.LineAlignment = [System.Drawing.StringAlignment]::Center
                $r = New-Object System.Drawing.RectangleF 0, 0, $size, $size
                $g.DrawString('IG', $font, [System.Drawing.Brushes]::White, $r, $sf)
            }
            finally { $sf.Dispose() }
        }
        finally { $font.Dispose() }
    }
    finally { $g.Dispose() }

    # Клон: сохранение «сырого» FromHandle(GetHicon) иногда падает, пока живёт Bitmap.
    $hIcon = $bmp.GetHicon()
    $iconTmp = [System.Drawing.Icon]::FromHandle($hIcon)
    $icon = $null
    try {
        $icon = New-Object System.Drawing.Icon -ArgumentList @($iconTmp, $size, $size)
    }
    finally {
        $iconTmp.Dispose()
    }

    if ($null -ne $icon) {
        try {
            $fs = New-Object System.IO.FileStream($out, [System.IO.FileMode]::Create, [System.IO.FileAccess]::Write)
            try { $icon.Save($fs) }
            finally { $fs.Dispose() }
        }
        finally { $icon.Dispose() }
    }
}
finally {
    if ($null -ne $bmp) { $bmp.Dispose() }
}
