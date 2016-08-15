@setlocal

@rem Delete output directory
rmdir /S /Q netstandard1.3
rmdir /S /Q nupkgs

@rem Clean project
rmdir /S /Q .\src\Asn1Net\bin
rmdir /S /Q .\src\Asn1Net\obj
del .\src\Asn1Net\project.lock.json
rmdir /S /Q .\test\Asn1Net.Test\bin
rmdir /S /Q .\test\Asn1Net.Test\obj
del .\test\Asn1Net.Test\project.lock.json

@rem Build project
dotnet restore .\src\Asn1Net\ || goto :error
dotnet build .\src\Asn1Net\ --configuration Release || goto :error

dotnet restore .\test\Asn1Net.Test\ || goto :error
dotnet build .\test\Asn1Net.Test\ --configuration Release || goto :error
@rem CI will run tests
@rem dotnet test .\test\Asn1Net.Test\ || goto :error

@rem Copy result to output directory
mkdir netstandard1.3 || goto :error
copy .\src\Asn1Net\bin\Release\netstandard1.3\Asn1Net.dll .\netstandard1.3 || goto :error
copy .\src\Asn1Net\bin\Release\netstandard1.3\Asn1Net.xml .\netstandard1.3 || goto :error

@rem Create nuget package
mkdir nupkgs || goto :error
dotnet pack .\src\Asn1Net\ --configuration Release --output .\nupkgs || goto :error

@echo *** BUILD NETSTANDARD1.3 SUCCESSFUL ***
@endlocal
@exit /b 0

:error
@echo *** BUILD NETSTANDARD1.3 FAILED ***
@endlocal
@exit /b 1