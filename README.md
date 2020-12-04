# SpeedRunDotCom-Console
C# console application that queries information from speedrun.com service. The application uses the library [SpeedrunComSharp](https://github.com/LiveSplit/SpeedrunComSharp) and must be downloaded separately to build.

## Command Line Arguments
### All Required:
  -g GAME NAME, --Game=GAME NAME                    Required.

  -c ANY%/100%/etc.., --Category=ANY%/100%/etc..    Required. The specific category for the game

### Either Required:
  -w, --WorldRecord                                 Whether to show the world record time or not

  -u USERNAME, --Username=USERNAME                  Username to look up record for

### Optional:
  -p PC/PS4/etc.., --Platform=PC/PS4/etc..          The specific platform for the game

  --f1 FILTERNAME & --v1 FILTERVALUE                The 1st name & value of a variable to filter on

  --f2 FILTERNAME & --v2 FILTERVALUE                The 2nd name & value of a variable to filter on

  --f3 FILTERNAME & --v3 FILTERVALUE                The 3rd name & value of a variable to filter on

### Output
  -o, --OutputToFile                                Output results to "output.txt" file in executing location

### Miscellanous
  --help                                            Display this help screen.

  --version                                         Display version information.

## Samples

-g CrossCode -c Any% NMS -w
> WR: 02:04:43 - Symphonian46

-g CrossCode -c Any% NMS -p Xbox One -w
> WR: 04:34:58 - SaviorXTanren

-g CrossCode -c Any% NMS -u SaviorXTanren
> PB: 04:34:58 - SaviorXTanren

-g CrossCode -c Any% NMS -w -u SaviorXTanren
> WR: 02:04:43 - Symphonian46, PB: 04:34:58 - SaviorXTanren

-g Resident Evil 3: Nemesis -c PS -w
> WR: 01:07:18 - yesalexx

-g Super Mario 64 -c 120 Star -w
> WR: 01:38:28 - simply

-g New Super Mario Bros. -c Any% -w
> WR: 00:22:37.0330000 - Glitchman24

-g New Super Mario Bros. -c Any% -w --f1 Release --v1 Emulator
> WR: 00:23:29 - ILoveSMB
