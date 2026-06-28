# TODO app for radiologists

This repo contains my submission for a take-home assignment I got a job application.

The requirements for the app were:
* Small to-do task management API and frontend
* Backend uses .NET Core
* Database uses EF Core in-memory or SQLite
* Frontend uses React or Vue

This README.md is a high-level description of my approach to this assignment.
For a more "raw", stream-of-consciousness record of what I did, see NOTES.txt.
For low-level, but "curated" (not stream-of-consciousness) descriptions of
my solution, see the comments in:
* backend/Backend.cs

NOTE: I didn't use an LLM for any of this: in particular, all words in the
documentation & comments are my own.

NOTE: I didn't use an IDE for any of this, which works fine for Python (long
live the REPL, particularly iPython), but doesn't work so well for C#.
I tried to find a REPL for it, but that... doesn't seem to be a thing.
There's `dotnet-repl`, which is someone's side project, and is okay for
experimenting with basic language functionality, but not much more than that.
If I were to be working with .NET a lot, I would switch to VSCode or
something.


## The plan

I had never used .NET Core before, and I hadn't done UI stuff in a while, so
this assignment was a chance to dust off some old skills and learn some new
ones.

I had some experience with the Microsoft stack, all the way from VB6 and
Classic ASP, through VB.NET and C# and ASP.NET.
So I understood the C# language well enough, but had only ever worked on
existing codebases, never created one from scratch.
I have lots of experience with creating Python web services from scratch,
using e.g. Django and Flask.
So my plan was to approach .NET Core by thinking about what I would do in
Django, and then finding tutorials and documentation for the .NET Core
equivalents.

The requirements didn't say much about the TODO app's design or functionality.
I wanted to show some creativity if possible, so I thought about what the company
which I was applying was doing: "AI-driven MRI scans".
I imagined a TODO app for radiologists, where each TODO item has an associated MRI
image, and asks the radiologist to outline things within it (tumors, etc).
The outlining could be done with a simple <canvas>-based component, allowing the
user to draw red lines on the image.
Also, I had worked previously with HL7 (a health industry data exchange protocol),
and saw HL7 listed as a nice-to-have in the job description, so I imagined an
HL7 "listener" service which would generate TODO items from HL7 messages.


## Learning .NET Core

Microsoft's official docs are fantastic, so I basically just used those.
There was the occasional case of googling a weird error message and finding
a StackOverflow answer for it.
I used `dotnet new webapi` as the basis for my backend program, which gave me
a .csproj, some configuration stuff like launch settings, and a Program.cs
entry point.
I ended up totally rewriting Program.cs, and renaming it to Backend.cs.
See that file's comments for further details.


## Making the React app

I've done some React in the past, but never created a React app from scratch.
I used `npx create-react-app` for the basic scaffold, then ...TODO
