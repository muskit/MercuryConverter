# Preparing files
This is a guide to populating the `data` working folder found in this application's directory, which will eventually contain all relevant data for repacking & converting charts. You may set up this folder elsewhere for storage space reasons. We will refer to this working folder as `data` throughout the guide.

**This project will only repack audio on Reverse 3.07 properly.**

## Table of Contents (sorted by descending time consumption)
1. [Song Audio](#song-audio-datamer_bgm)
2. [Jackets](#jackets-datajackets)
3. [Metadata](#metadata-datamusicparametertable)
4. [Charts](#charts-datamusicdata)

## Song Audio (`data/MER_BGM`)
*~18.8 GB for WAVs*

Due to the audio indexing data in this project only done for **Reverse 3.07**, this process will only work for files of that version.

You will need the latest version of [Audio Cue Editor (ACE)](https://github.com/LazyBone152/ACE).

For each of the files below located in `<WAC>/app/WindowsNoEditor/Mercury/Content/Sound/Bgm`...

- MER_BGM.awb
- MER_BGM_V3_01.awb
- MER_BGM_V3_02.awb
- MER_BGM_V3_03.awb
- MER_BGM_V3_04.awb
- MER_BGM_V3_05.awb
- MER_BGM_V3_06.awb
- MER_BGM_V3_07.awb

...follow these steps on each file:

1. Load the file in ACE using "File > Load (AWB)".
    - If asked to open the matching ACB, click "No".
2. Export all of the AWB's streams using "Tools > Extract All (wav)" into a folder in `data/MER_BGM` depending on the current AWB file according to the table:

| AWB File          | Folder in MER_BGM |
|-------------------|-------------------|
| MER_BGM.awb       | MER               |
| MER_BGM_V3_01.awb | 01                |
| MER_BGM_V3_02.awb | 02                |
| MER_BGM_V3_03.awb | 03                |
| MER_BGM_V3_04.awb | 04                |
| MER_BGM_V3_05.awb | 05                |
| MER_BGM_V3_06.awb | 06                |
| MER_BGM_V3_07.awb | 07                |

## Jackets (`data/jackets`)
*~54.4 MB*

For this, you will need [UE Viewer](https://www.gildor.org/en/projects/umodel).

1. Run `umodel_64.exe` and configure its Startup Options.
    - Set "Path to game files" to `<WAC>/app/WindowsNoEditor/Mercury/Content/UI/Textures/JACKET`.
    - Enable "Override game detection" and set it to "Unreal engine 4.19".
    - Click OK.
2. In the left panel, right click on "All packages", then click on "Export folder content".
    - Under "Texture Export," set format to PNG, and the path to `data/jackets`.
    - Click OK to begin exporting jacket images.

## Metadata (`data/MusicParameterTable.*`)
*<1 MB*

In `<WAC>/app/WindowsNoEditor/Mercury/Content/Table/`, simply copy `MusicParameterTable.uasset` and `MusicParameterTable.uexp` into `data`.

## Charts (`data/MusicData`)
*~59.5 MB*
Simply copy the `MusicData` folder at `<WAC>/app/WindowsNoEditor/Mercury/Content/` into `data`.
