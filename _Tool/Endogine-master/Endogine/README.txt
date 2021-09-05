**  COMPILING ENDOGINE  **

Pre-requisites
--------------

---Developing---
* VS2005 or Visual C# Express (free). SharpDevelop 2.0 should work too, but I haven't tried it.
* DirectX 9.0c
* Managed extensions for DirectX, feb 2006 or later


---Runtime---
* DirectX 9.0c
* Managed extensions for DirectX, feb 2006 or later
* .NET 2.0

!!!!
Note that on some computers you might have to disable the MDX 2.0 dll:
WINDOWS\Microsoft.NET\DirectX for Managed Code\2.0.0.0_x86\Microsoft.DirectX.dll
The error is something like "ambiguous reference"
I renamed it to .dllx and then it worked.
!!!!


---OpenGL rendering---
Tao framework. I haven't come so far with the renderer plugin yet though.


---Sound playback---
Bass library (download shareware license from the Endogine page at CodeProject)
There's a DirectX player plugin included that has no external dependencies, but I haven't tested it in quite a while. Stay with Bass for now.



Running the Tests example project
---------------------------------
* Make sure Endogine.Editors.dll is present in the .exe folder. To use sound, include Endogine.Audio.Bass.dll and the files in http://www.codeproject.com/csharp/Endogine/Bass.zip (shareware license)

* Open Tests.sln in VS2005.

* Right-click on the Tests project and select Set as Start-up project

* From the Debug menu, choose Exceptions... Expand Managed Debugging Assistants, and uncheck LoaderLock / Throw.

* If the project Endogine.Renderer.OpenGL is included in the solution, you MUST delete it or unload it, otherwise the solution won't compile. (The same might go for Endogine.Audio.Munoz)

* Start, click OK in the Setup window, and click the Main window. You're in!

Try the tests from the Engine Tests menu. Select once to start, select again to shut them down.