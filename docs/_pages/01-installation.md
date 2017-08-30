---
layout: default
title: Installation
permalink: /installation/index.html
---

## <a name="requirements" title="Requirements"></a>Requirements

> Ditto has been developed against **Umbraco v7.3.2** and will support that version and above.

Ditto can be installed from either [Our Umbraco](#our-umbraco) or [NuGet](#nuget) package repositories, or build [manually from the source-code](#manual).

---

## <a name="our-umbraco" title="Our Umbraco Installation"></a>Our Umbraco package repository

To install from Our Umbraco, please download the package from:

> [http://our.umbraco.org/projects/developer-tools/ditto](http://our.umbraco.org/projects/developer-tools/ditto) 

---

## <a name="nuget" title="Nuget Installation"></a>NuGet package repository

To [install from NuGet](https://www.nuget.org/packages/Our.Umbraco.Ditto), you can run the following command from within Visual Studio:

```
PM> Install-Package Our.Umbraco.Ditto
```

We also have a [MyGet package repository](https://www.myget.org/gallery/umbraco-packages) - for bleeding-edge / development releases.

---

## <a name="manual" title="Manual Installation"></a>Manual build

If you prefer, you can compile Ditto yourself, you'll need:

* [Visual Studio 2012 (or above, including Community Editions)](https://www.visualstudio.com/downloads/)
* Microsoft Build Tools 2013 (aka [MSBuild 12](https://www.microsoft.com/en-us/download/details.aspx?id=40760))

To clone it locally run the following git commands:

```
git clone https://github.com/leekelleher/umbraco-ditto.git umbraco-ditto
cd umbraco-ditto
.\build.cmd
```
