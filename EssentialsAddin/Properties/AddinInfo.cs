﻿using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly: Addin(
	"EssentialsAddin",
	Namespace = "EssentialsAddin",
	Version = "1.5.0"
)]

[assembly: AddinName("EssentialsAddin")]
[assembly: AddinCategory("IDE extensions")]
[assembly: AddinDescription("EssentialsAddin provides:\n "+
    " - Solution tree filter\n" +
    " - One click to open file functionality. \n" +
    " - Expand project filter when closing all projects")]
[assembly: AddinAuthor("Ivo Krugers")]
