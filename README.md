# totp-generator
Generate a TOTP token (2-Step Verification) using secret key from services like google, dropbox, etc.

## Details
- Simple implementation of the RFC 6238 (https://tools.ietf.org/html/rfc6238) in C# WPF Application.
- Secret keys are by default stored in "MyDocuments/totp-generator-keys/Accounts.xml"'s file when using the save button.
- Secret key's minimal length is 3.

![alt tag](https://raw.githubusercontent.com/biji1/totp-generator/master/res/screen.jpg =500x)

## Todo
- Add settings for timer (default 30) and token's length (default 6) to make it works with other services like battle.net

## Done

02/27/16:
- Auto select the first entry
- Ask confirmation before deleting
- Added label "copied !" when OTP saved in clipboard

04/20/15:
- Button copy to clipboard added
- Fixed OTP beginning with 0
