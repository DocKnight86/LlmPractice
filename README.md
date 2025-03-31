# LlmPractice

Version 3

Local running large language models built into a Blazor App.

Requires .NET 9 and Docker running the Ollama docker image with llama3.1:8b and llama2:7b installed on port 11434

A future update will add support for deepseek-r1, gemma3 from Google, and phi4 from Microsoft. All open source.

https://hub.docker.com/r/ollama/ollama - Official Ollama Docker image.

All inspiration for this project came from: https://medium.com/scrum-and-coke/creating-a-web-api-with-net-9-to-interact-with-a-local-ollama-ai-instance-using-llama-3-1-41fcc3cceb8b

Quite a bit had already changed since the article was written, particularly with the Microsoft.Extensions.AI package, so it wasn’t as simple as copying and pasting to get it working but the article pointed me in the right direction and got me thinking!

-------------------------------------------------------------------------------------------------------------------

**Change Logs**

*Version 2 to version 3:*
- Now remembers parts of the conversation history so the models have some recall and ability to go back for context.
- Cleaned up some unused code.
- Fixed warnings produced on compile.
- Added asynchronous logging of chat messages to a local NDJSON file.
  - File can be found in the ChatHistory folder which will be created when first prompting the bot.
  - Will eventually allow for the reloading of past conversations or using ML.NET machine learning to better tokenize long running chats.
  - Will eventually be moved to a database, but this is easier for local running of the application.
- Added AppendChatMessageAsync function to serialize messages as JSON and append them to the file.

-------------------------------------------------------------------------------------------------------------------

![Screenshot 2025-03-25 122451](https://github.com/user-attachments/assets/62938a9a-a9f0-4513-8f6f-f32921b6cb9d)

![Screenshot 2025-03-25 122819](https://github.com/user-attachments/assets/c585612e-795b-441a-9de1-2ec21e0bb495)

**Watch the demo videos here:**

Version 3 Video (NEWEST)

[![IMAGE ALT TEXT HERE](https://img.youtube.com/vi/mo-5q1rvdpE/0.jpg)](https://www.youtube.com/watch?v=mo-5q1rvdpE)


Version 2 Video (old)

[![IMAGE ALT TEXT HERE](https://img.youtube.com/vi/FcH_w3bsdZQ/0.jpg)](https://www.youtube.com/watch?v=FcH_w3bsdZQ)


Version 1 Video (old)

[![IMAGE ALT TEXT HERE](https://img.youtube.com/vi/6Y4LnnlxGQk/0.jpg)](https://www.youtube.com/watch?v=6Y4LnnlxGQk)
