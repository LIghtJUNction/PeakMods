# Changelog

- 1.0.1
- Added command: /hide : hide chat box immediately

- 1.1.4
- added command: /ai
usage: /ai [prompt] @send
: send prompt to OpenAI chat completion API and get response,send to other players if @send is specified
: if @send is not specified, only send to yourself

-added config: autoTranslate
: if true, automatically translate received messages to your language using OpenAI API.(need test,default: false)
