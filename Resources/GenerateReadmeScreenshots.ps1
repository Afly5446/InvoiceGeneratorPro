# Synthetic PNG previews for README (replace with real captures when publishing).
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

function Save-LabeledPng {
    param(
        [int]$Width,
        [int]$Height,
        [string]$Title,
        [string]$Subtitle,
        [string]$OutPath
    )
    $bmp = $null
    try {
        $bmp = New-Object System.Drawing.Bitmap $Width, $Height
        $g = [System.Drawing.Graphics]::FromImage($bmp)
        try {
            $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
            $g.Clear([System.Drawing.Color]::FromArgb(255, 30, 27, 75))
            $brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush (
                [System.Drawing.Rectangle]::new(0, 0, $Width, 120),
                [System.Drawing.Color]::FromArgb(255, 79, 70, 229),
                [System.Drawing.Color]::FromArgb(255, 49, 46, 164),
                [System.Drawing.Drawing2D.LinearGradientMode]::Vertical
            )
            try { $g.FillRectangle($brush, 0, 0, $Width, 120) }
            finally { $brush.Dispose() }

            $fontTitle = New-Object System.Drawing.Font -ArgumentList @('Segoe UI', [single]22, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
            try {
                $fontSub = New-Object System.Drawing.Font -ArgumentList @('Segoe UI', [single]12, [System.Drawing.FontStyle]::Regular, [System.Drawing.GraphicsUnit]::Pixel)
                try {
                    $w = [System.Drawing.Brushes]::White
                    $lg = [System.Drawing.Brushes]::LightGray
                    $g.DrawString($Title, $fontTitle, $w, 24, 28)
                    $g.DrawString($Subtitle, $fontSub, $lg, 24, 62)
                    $g.DrawString('Placeholder — replace with real screenshot (see docs/screenshots/README.md)', $fontSub, $lg, 24, [float]($Height - 48))
                }
                finally { $fontSub.Dispose() }
            }
            finally { $fontTitle.Dispose() }
        }
        finally { $g.Dispose() }
        $bmp.Save($OutPath, [System.Drawing.Imaging.ImageFormat]::Png)
    }
    finally {
        if ($null -ne $bmp) { $bmp.Dispose() }
    }
}

$projectRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$shots = Join-Path $projectRoot 'docs\screenshots'
if (-not (Test-Path $shots)) { New-Item -ItemType Directory -Path $shots -Force | Out-Null }

Save-LabeledPng -Width 920 -Height 520 -Title 'Invoice Generator Pro' -Subtitle 'Main window (synthetic preview)' -OutPath (Join-Path $shots 'main-window.png')
Save-LabeledPng -Width 920 -Height 520 -Title 'Form & validation' -Subtitle 'WPF bindings + IDataErrorInfo' -OutPath (Join-Path $shots 'form-validation.png')
Save-LabeledPng -Width 920 -Height 520 -Title 'Export to PDF' -Subtitle 'PdfSharp + print preview' -OutPath (Join-Path $shots 'pdf-export.png')

Write-Host "Wrote PNGs to $shots"
