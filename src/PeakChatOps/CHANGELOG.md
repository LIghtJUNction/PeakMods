# Changelog

- 1.2.0
- **UI Improvements:**
  - Fixed chat panel occupying entire screen, now only uses actual panel size
  - Fixed input text appearing white and hard to read - now uses proper contrast
  - Improved maximize/minimize functionality using USS styles instead of hardcoded values
  - Fixed long text overflowing horizontally - now properly wraps to new lines

- **Message System Fixes:**
  - Fixed remote player messages not displaying properly
  - Fixed null reference exception in `/dev mock player` command
  - Improved message receiving and display pipeline

- **Code Quality:**
  - Removed unnecessary thread switching calls
  - Enhanced error handling and debug logging
  - Fixed compilation errors

- 1.1.4
- added command: /ai
usage: /ai [prompt] @send
: send prompt to OpenAI chat completion API and get response,send to other players if @send is specified
: if @send is not specified, only send to yourself

-added config: autoTranslate
: if true, automatically translate received messages to your language using OpenAI API.(need test,default: false)

- 1.0.1
- Added command: /hide : hide chat box immediately

