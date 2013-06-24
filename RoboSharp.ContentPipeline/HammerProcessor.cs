using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

// TODO: replace these with the processor input and output types.
using TInput = System.String;
using TOutput = RoboSharp.Library.Level;
using RoboSharp.Library;
using System.Reflection;
using NUnit.Framework;

namespace RoboSharp.ContentPipeline
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "Hammer Processor")]
    public class HammerProcessor : ContentProcessor<TInput, TOutput>
    {
        private enum ParseState
        {
            KEY,
            START_SECTION,
            SECTION,
            IGNORE
        }

        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            TOutput level = new TOutput();

            string[] lines = input.Split(new [] {"\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);

            ParseState state = ParseState.KEY;

            //The object that we are updating
            object currentObject = null;

            //The method that we will use to set the object
            MethodInfo currentMethod = null;

            foreach (var line in lines)
            {
                switch (state)
                {
                    case ParseState.KEY:

                        currentMethod = GetCurrentMethod(level, line);

                        if (currentMethod != null)
                        {
                            currentObject = GetCurrentObject(currentMethod, line);
                            state = ParseState.START_SECTION;
                        }
                        else
                        {
                            state = ParseState.IGNORE;
                        }
                        break;
                    case ParseState.START_SECTION:
                        if (line.Trim().StartsWith("{"))
                            state = ParseState.SECTION;
                        else
                            throw new FormatException();
                        break;
                    case ParseState.SECTION:

                        if (line.Trim().StartsWith("}"))
                        {
                            currentMethod.Invoke(level, new[] { currentObject });
                            state = ParseState.KEY;
                            continue;
                        }

                        string[] tokens = line.Trim().Replace("\"","").Split(new char [] {' '}, 2, StringSplitOptions.RemoveEmptyEntries);

                        MethodInfo method = null;

                        if (TryGetSetter(currentObject, tokens[0], out method))
                        {
                            InvokeMethod(currentObject, tokens[1], method);
                        }
                        break;
                    case ParseState.IGNORE:
                        if (line.Trim().StartsWith("}"))
                            state = ParseState.KEY;
                        break;
                    default:
                        break;
                }
            }


            return level;
        }

        private MethodInfo GetCurrentMethod(TOutput level, TInput line)
        {
            var trimmedLine = line.Trim();

            var attributedProperties = typeof(TOutput).GetProperties().FirstOrDefault(prop => IsSection(prop, trimmedLine));
            var attributedMethods = typeof(TOutput).GetMethods().FirstOrDefault(method => IsSection(method, trimmedLine));

            if (attributedProperties != null)
            {
                return attributedProperties.GetSetMethod();
            }
            else if (attributedMethods != null)
            {
                return attributedMethods;
            }
            else
            {
                return null;
            }
        }

        private void InvokeMethod(object currentObject, TInput p, MethodInfo method)
        {
            method.Invoke(currentObject, new object[] { Convert.ChangeType(p, method.GetParameters().First().ParameterType) });
        }

        private bool TryGetSetter(object obj, TInput methodName, out MethodInfo method)
        {
            var property = obj.GetType().GetProperties().FirstOrDefault(prop => prop.GetCustomAttributes(typeof(KeyAttribute), true).OfType<KeyAttribute>().Any(attr => attr.Key.Equals(methodName)));

            method = null;

            if (property != null)
            {
                method = property.GetSetMethod();
                return true;
            }

            return false;
        }

        private object GetCurrentObject(MethodInfo setterMethod, TInput line)
        {
            return Activator.CreateInstance(setterMethod.GetParameters()[0].ParameterType);
        }

        private bool IsSection(MemberInfo method, string key)
        {
            return method.GetCustomAttributes(typeof(SectionAttribute), true).OfType<SectionAttribute>().Any(attr => attr.SectionName.Equals(key));
        }
    }
}

namespace RoboSharp.ContentPipeline.Tests
{
    [TestFixture]
    public class PipelineTests {
    
        [Test]
        public void TestSimpleProcessor()
        {
            HammerProcessor processor = new HammerProcessor();

            TInput testString = 
                "versioninfo\n" +
                "{\n" +
                "    \"editorversion\" \"400\"\n" +
                "    \"editorbuild\" \"5004\"\n" +
                "    \"mapversion\" \"20\"\n" +
                "    \"formatversion\" \"100\"\n" +
                "    \"prefab\" \"0\"\n" +
                "}";

            TOutput level = processor.Process(testString, null);

            Assert.NotNull(level.VersionInfo);
            Assert.AreEqual(5004, level.VersionInfo.EditorBuild);

        }

        [Test]
        public void TestWithDummyFile()
        {
            HammerProcessor processor = new HammerProcessor();

            TOutput level = processor.Process(new HammerImporter().Import("C:\\Users\\Paul\\Documents\\Visual Studio 2010\\Projects\\RoboSharp\\RoboSharp\\RoboSharpContent\\Dummy_Level.vmf", null), null);

            Assert.AreEqual(1, level.Entities.Count());
        }

    }
}