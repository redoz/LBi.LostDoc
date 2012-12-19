﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using LBi.Cli.Arguments;

namespace LBi.LostDoc.ConsoleApplication.Plugin.SinglePhase
{
    [ParameterSet("Single phase", Command = "Template", HelpMessage = "Creates ldoc file and applies template")]
    public class SinglePhaseCommand : ICommand
    {
        [Parameter(HelpMessage = "Template name"), Required]
        public string Template { get; set; }

        [Parameter(HelpMessage = "Assemblies"), Required]
        public string[] Path { get; set; }

        [Parameter(HelpMessage = "Output directory"), Required]
        public string Output{ get; set; }

        [Parameter(HelpMessage = "Optional template arguments.")]
        public Dictionary<string, object> Arguments { get; set; }

        [Parameter(HelpMessage = "Include non-public members.")]
        public LBi.Cli.Arguments.Switch IncludeNonPublic { get; set; }

        [Parameter(HelpMessage = "Includes doc comments from the BCL for referenced types.")]
        public LBi.Cli.Arguments.Switch IncludeBclDocComments { get; set; }

        [Parameter(HelpMessage = "Path to xml containing additional comments for Assembly and Namespaces.")]
        public string NamespaceDocPath { get; set; }

        [Parameter(HelpMessage = "Type name filter (Compared against the type's FullName, including Namespace).")]
        public string Filter { get; set; }

        [Parameter(HelpMessage = "Include verbose output.")]
        public Switch Verbose { get; set; }

        public void Invoke()
        {
            
            List<string> ldocFiles = new List<string>();

            foreach (var path in this.Path)
            {
                ExtractCommand extract = new ExtractCommand();
                extract.IncludeNonPublic = this.IncludeNonPublic;
                extract.Filter = this.Filter;
                extract.IncludeBclDocComments = this.IncludeBclDocComments;
                extract.NamespaceDocPath = this.NamespaceDocPath;
                extract.Path = path;
                extract.Invoke();
                ldocFiles.Add(extract.Output);
            }

            string tempFolder = System.IO.Path.GetTempPath();
            tempFolder = System.IO.Path.Combine(tempFolder, "ldoc_{yyyyMMddHHmmss}");
            Directory.CreateDirectory(tempFolder);
            foreach (var ldocFile in ldocFiles)
            {
                File.Copy(ldocFile, System.IO.Path.Combine(tempFolder, System.IO.Path.GetFileName(ldocFile)));
            }

            TemplateCommand template = new TemplateCommand();
            template.Path = tempFolder;
            template.Arguments = this.Arguments;
            template.IgnoreVersionComponent = null;
            template.Output = this.Output;
            template.Template = this.Template;
            template.Verbose = this.Verbose;
            template.Invoke();
        }
    }
}