using System.Collections.Generic;
using System.Linq;
using DeepEqual.Syntax;
using NUnit.Framework;
using Veil.Parser;
using Veil.Parser.Nodes;
using Veil.SuperSimple;

namespace Veil.Tests.SuperSimple
{
    [TestFixture]
    internal class SuperSimpleParserTests : ParserTestBase<SuperSimpleParser>
    {
        [Test]
        public void Should_not_choke_on_an_email_address()
        {
            var input = "<a href=\"mailto:foo@foo.com\">foo@foo.com</a>";
            var output = Parse(input, typeof(object));
            AssertSyntaxTree(
                output,
                SyntaxTree.WriteString("<a href=\"mailto:foo@foo.com\">foo@foo.com</a>")
            );
        }

        [Test]
        public void Should_replace_primitive_model_with_value()
        {
            var input = @"<html><head></head><body>Hello there @Model;</body></html>";
            var output = Parse(input, typeof(string));
            AssertSyntaxTree(output,
                SyntaxTree.WriteString("<html><head></head><body>Hello there "),
                SyntaxTree.WriteExpression(SyntaxTreeExpression.Self(typeof(string), ExpressionScope.RootModel)),
                SyntaxTree.WriteString("</body></html>")
            );
        }

        [Test]
        public void Should_replaces_valid_property()
        {
            var input = @"<html><head></head><body>Hello there @Model.Name;</body></html>";
            var model = new { Name = "bob" };
            var output = Parse(input, model.GetType());
            AssertSyntaxTree(output,
                SyntaxTree.WriteString("<html><head></head><body>Hello there "),
                SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.GetType(), "Name", ExpressionScope.RootModel)),
                SyntaxTree.WriteString("</body></html>")
            );
        }

        [Test]
        public void Should_replace_multiple_properties_with_the_same_name()
        {
            const string input = @"<html><head></head><body>Hello there @Model.Name;, nice to see you @Model.Name;</body></html>";
            var model = new { Name = "bob" };
            var output = Parse(input, model.GetType());
            AssertSyntaxTree(output,
                SyntaxTree.WriteString("<html><head></head><body>Hello there "),
                SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.GetType(), "Name", ExpressionScope.RootModel)),
                SyntaxTree.WriteString(", nice to see you "),
                SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.GetType(), "Name", ExpressionScope.RootModel)),
                SyntaxTree.WriteString("</body></html>")
            );
        }

        [Test]
        public void Should_replace_multiple_properties_from_model()
        {
            var input = @"<html><head></head><body>Hello there @Model.Name; - welcome to @Model.SiteName;</body></html>";
            var model = new
            {
                Name = "Bob",
                SiteName = "Cool Site!"
            };
            var output = Parse(input, model.GetType());

            AssertSyntaxTree(output,
                SyntaxTree.WriteString("<html><head></head><body>Hello there "),
                SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.GetType(), "Name", ExpressionScope.RootModel)),
                SyntaxTree.WriteString(" - welcome to "),
                SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.GetType(), "SiteName", ExpressionScope.RootModel)),
                SyntaxTree.WriteString("</body></html>")
            );
        }

        [Test]
        public void Should_support_implicit_model_reference_for_each_block()
        {
            var input = @"<html><head></head><body><ul>@Each.Users;<li>@Current;</li>@EndEach;</ul></body></html>";
            var model = new { Users = new List<string>() { "Bob", "Jim", "Bill" } };
            var output = Parse(input, model.GetType());
            AssertSyntaxTree(output,
                SyntaxTree.WriteString("<html><head></head><body><ul>"),
                SyntaxTree.Iterate(
                    SyntaxTreeExpression.Property(model.GetType(), "Users"),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("<li>"),
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Self(typeof(string), ExpressionScope.CurrentModelOnStack)),
                        SyntaxTree.WriteString("</li>")
                    )
                ),
                SyntaxTree.WriteString("</ul></body></html>")
            );
        }

        [Test]
        public void Should_support_explicit_model_reference_for_each_block()
        {
            var input = @"<html><head></head><body><ul>@Each.Model.Users;<li>@Current;</li>@EndEach;</ul></body></html>";
            var model = new { Users = new List<string>() { "Bob", "Jim", "Bill" } };
            var output = Parse(input, model.GetType());
            AssertSyntaxTree(output,
                SyntaxTree.WriteString("<html><head></head><body><ul>"),
                SyntaxTree.Iterate(
                    SyntaxTreeExpression.Property(model.GetType(), "Users", ExpressionScope.RootModel),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("<li>"),
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Self(typeof(string), ExpressionScope.CurrentModelOnStack)),
                        SyntaxTree.WriteString("</li>")
                    )
                ),
                SyntaxTree.WriteString("</ul></body></html>")
            );
        }

        [Test]
        public void Should_use_multiple_current_statements_inside_each()
        {
            const string input = @"<html><head></head><body><ul>@Each.Users;<li id=""@Current;"">@Current;</li>@EndEach;</ul></body></html>";
            var model = new { Users = new List<string>() { "Bob", "Jim", "Bill" } };
            var output = Parse(input, model.GetType());

            AssertSyntaxTree(output,
                SyntaxTree.WriteString("<html><head></head><body><ul>"),
                SyntaxTree.Iterate(
                    SyntaxTreeExpression.Property(model.GetType(), "Users"),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("<li id=\""),
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Self(typeof(string), ExpressionScope.CurrentModelOnStack)),
                        SyntaxTree.WriteString("\">"),
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Self(typeof(string), ExpressionScope.CurrentModelOnStack)),
                        SyntaxTree.WriteString("</li>")
                    )
                ),
                SyntaxTree.WriteString("</ul></body></html>")
            );
        }

        [Test]
        public void Should_throw_if_using_non_enumerable_type_for_each()
        {
            var input = @"<html><head></head><body><ul>@Each.Users;<li id=""@Current;"">@Current;</li>@EndEach;</ul></body></html>";
            var model = new { Users = new { } };
            Assert.Throws<VeilParserException>(() =>
            {
                Parse(input, model.GetType());
            });
        }

        [Test]
        public void Should_combine_single_substitutions_and_each_substitutions()
        {
            var input = @"<html><head></head><body><ul>@Each.Users;<li>Hello @Current;, @Model.Name; says hello!</li>@EndEach;</ul></body></html>";
            var model = new
            {
                Name = "Nancy",
                Users = new List<string>() { "Bob", "Jim", "Bill" }
            };
            var output = Parse(input, model.GetType());

            AssertSyntaxTree(output,
                SyntaxTree.WriteString("<html><head></head><body><ul>"),
                SyntaxTree.Iterate(
                    SyntaxTreeExpression.Property(model.GetType(), "Users"),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("<li>Hello "),
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Self(typeof(string), ExpressionScope.CurrentModelOnStack)),
                        SyntaxTree.WriteString(", "),
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.GetType(), "Name", ExpressionScope.RootModel)),
                        SyntaxTree.WriteString(" says hello!</li>")
                    )
                ),
                SyntaxTree.WriteString("</ul></body></html>")
            );
        }

        [Test]
        public void Should_allow_model_statement_to_be_followed_by_a_newline()
        {
            var input = "<html><head></head><body>Hello there @Model.Name;\n</body></html>";
            var model = new { Name = "Bob" };
            var output = Parse(input, model.GetType());
            AssertSyntaxTree(output,
                SyntaxTree.WriteString("<html><head></head><body>Hello there "),
                SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.GetType(), "Name", ExpressionScope.RootModel)),
                SyntaxTree.WriteString("\n</body></html>")
            );
        }

        [Test]
        public void Should_allow_each_statements_to_work_over_multiple_lines()
        {
            var input = "<html>\n\t<head>\n\t</head>\n\t<body>\n\t\t<ul>@Each.Users;\n\t\t\t<li>@Current;</li>@EndEach;\n\t\t</ul>\n\t</body>\n</html>";
            var model = new { Users = new List<string>() { "Bob", "Jim", "Bill" } };
            var output = Parse(input, model.GetType());

            AssertSyntaxTree(output,
                SyntaxTree.WriteString("<html>\n\t<head>\n\t</head>\n\t<body>\n\t\t<ul>"),
                SyntaxTree.Iterate(
                    SyntaxTreeExpression.Property(model.GetType(), "Users"),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("\n\t\t\t<li>"),
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Self(typeof(string))),
                        SyntaxTree.WriteString("</li>")
                    )
                ),
                SyntaxTree.WriteString("\n\t\t</ul>\n\t</body>\n</html>")
            );
        }

        [Test]
        public void Should_parse_if_block_with_implcit_model_reference()
        {
            var input = @"<html><head></head><body>@If.HasUsers;<ul>@Each.Users;<li>Hello @Current;, @Model.Name; says hello!</li>@EndEach;</ul>@EndIf;</body></html>";
            var model = new FakeModel("Nancy", new List<string>() { "Bob", "Jim", "Bill" });
            var output = Parse(input, model.GetType());

            AssertSyntaxTree(output,
                SyntaxTree.WriteString("<html><head></head><body>"),
                SyntaxTree.Conditional(
                    SyntaxTreeExpression.Property(model.GetType(), "HasUsers"),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("<ul>"),
                        SyntaxTree.Iterate(
                            SyntaxTreeExpression.Property(model.GetType(), "Users"),
                            SyntaxTree.Block(
                                SyntaxTree.WriteString("<li>Hello "),
                                SyntaxTree.WriteExpression(SyntaxTreeExpression.Self(typeof(string))),
                                SyntaxTree.WriteString(", "),
                                SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.GetType(), "Name", ExpressionScope.RootModel)),
                                SyntaxTree.WriteString(" says hello!</li>")
                            )
                        ),
                        SyntaxTree.WriteString("</ul>")
                    )
                ),
                SyntaxTree.WriteString("</body></html>")
            );
        }

        [Test]
        public void Should_parse_if_block_with_explicit_model_reference()
        {
            var input = @"<html><head></head><body>@If.Model.HasUsers;<ul>@Each.Users;<li>Hello @Current;, @Model.Name; says hello!</li>@EndEach;</ul>@EndIf;</body></html>";
            var model = new FakeModel("Nancy", new List<string>() { "Bob", "Jim", "Bill" });
            var output = Parse(input, model.GetType());

            AssertSyntaxTree(output,
                SyntaxTree.WriteString("<html><head></head><body>"),
                SyntaxTree.Conditional(
                    SyntaxTreeExpression.Property(model.GetType(), "HasUsers", ExpressionScope.RootModel),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("<ul>"),
                        SyntaxTree.Iterate(
                            SyntaxTreeExpression.Property(model.GetType(), "Users"),
                            SyntaxTree.Block(
                                SyntaxTree.WriteString("<li>Hello "),
                                SyntaxTree.WriteExpression(SyntaxTreeExpression.Self(typeof(string))),
                                SyntaxTree.WriteString(", "),
                                SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.GetType(), "Name", ExpressionScope.RootModel)),
                                SyntaxTree.WriteString(" says hello!</li>")
                            )
                        ),
                        SyntaxTree.WriteString("</ul>")
                    )
                ),
                SyntaxTree.WriteString("</body></html>")
            );
        }

        [Test]
        public void Should_not_render_block_when_ifnot_statements_returns_true()
        {
            var input = @"<html><head></head><body>@IfNot.HasUsers;<p>No users found!</p>@EndIf;<ul>@Each.Users;<li>Hello @Current;, @Model.Name; says hello!</li>@EndEach;</ul></body></html>";
            var model = new FakeModel("Nancy", new List<string>() { "Bob", "Jim", "Bill" });
            var output = Parse(input, model.GetType());

            AssertSyntaxTree(output,
                SyntaxTree.WriteString("<html><head></head><body>"),
                SyntaxTree.Conditional(
                    SyntaxTreeExpression.Property(model.GetType(), "HasUsers"),
                    SyntaxTree.Block(),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("<p>No users found!</p>")
                    )
                ),
                SyntaxTree.WriteString("<ul>"),
                SyntaxTree.Iterate(
                    SyntaxTreeExpression.Property(model.GetType(), "Users"),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("<li>Hello "),
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Self(typeof(string))),
                        SyntaxTree.WriteString(", "),
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.GetType(), "Name", ExpressionScope.RootModel)),
                        SyntaxTree.WriteString(" says hello!</li>")
                    )
                ),
                SyntaxTree.WriteString("</ul></body></html>")
            );
        }

        [Test]
        public void Should_return_true_for_ifhascollection_when_if_model_has_a_collection_with_items_but_no_bool()
        {
            var input = @"<html><head></head><body>@If.HasUsers;<ul>@Each.Users;<li>Hello @Current;, @Model.Name; says hello!</li>@EndEach;</ul>@EndIf;</body></html>";
            var model = new { Users = new List<string>() { "Bob", "Jim", "Bill" }, Name = "Nancy" };
            var output = Parse(input, model.GetType());

            AssertSyntaxTree(
                output,
                SyntaxTree.WriteString("<html><head></head><body>"),
                SyntaxTree.Conditional(
                    SyntaxTreeExpression.HasItems(SyntaxTreeExpression.Property(model.GetType(), "Users")),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("<ul>"),
                        SyntaxTree.Iterate(
                            SyntaxTreeExpression.Property(model.GetType(), "Users"),
                            SyntaxTree.Block(
                                SyntaxTree.WriteString("<li>Hello "),
                                SyntaxTree.WriteExpression(SyntaxTreeExpression.Self(typeof(string))),
                                SyntaxTree.WriteString(", "),
                                SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.GetType(), "Name", ExpressionScope.RootModel)),
                                SyntaxTree.WriteString(" says hello!</li>")
                            )
                        ),
                        SyntaxTree.WriteString("</ul>")
                    )
                ),
                SyntaxTree.WriteString("</body></html>")
            );
        }

        [TestCase("Hello @Model.Name")]
        [TestCase("@Model.Name says hello")]
        [TestCase("Hello @Model.Name, welcome")]
        [TestCase("<b>Hello @Model.Name</b>")]
        [TestCase("Hello @Model.Name?")]
        [TestCase("Hello @Model.Name!")]
        [TestCase("Hello \"@Model.Name\"")]
        [TestCase("Hello '@Model.Name'")]
        public void Should_handle_expression_without_closing_tag(string input)
        {
            var model = new { Name = "Bob" };
            var template = Parse(input, model.GetType());
            var result = ((BlockNode)template).Nodes.OfType<WriteExpressionNode>().Single().Expression as PropertyExpressionNode;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.PropertyInfo, Is.EqualTo(model.GetType().GetProperty("Name")));
        }

        [TestCase(@"<html><head></head><body><ul>@Each;<li>Hello @Current;</li>@EndEach;</ul></body></html>")]
        [TestCase(@"<html><head></head><body><ul>@Each<li>Hello @Current</li>@EndEach</ul></body></html>")]
        public void Should_allow_each_without_a_variable_and_iterate_over_the_model_if_it_is_enumerable(string input)
        {
            var model = new List<string>() { "Bob", "Jim", "Bill" };
            var output = Parse(input, model.GetType());

            AssertSyntaxTree(
                output,
                SyntaxTree.WriteString("<html><head></head><body><ul>"),
                SyntaxTree.Iterate(
                    SyntaxTreeExpression.Self(model.GetType()),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("<li>Hello "),
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Self(typeof(string))),
                        SyntaxTree.WriteString("</li>")
                    )
                ),
                SyntaxTree.WriteString("</ul></body></html>")
            );
        }

        [Test]
        public void Model_with_exclaimation_should_html_encode()
        {
            var input = @"<html><head></head><body>Hello there @!Model.Name;</body></html>";
            var model = new { Name = "<b>Bob</b>" };
            var output = Parse(input, model.GetType());

            AssertSyntaxTree(
                output,
                SyntaxTree.WriteString("<html><head></head><body>Hello there "),
                SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.GetType(), "Name", ExpressionScope.RootModel), true),
                SyntaxTree.WriteString("</body></html>")
            );
        }

        [Test]
        public void Current_with_exclaimation_and_no_parameters_should_html_encode()
        {
            var input = @"<html><head></head><body><ul>@Each;<li>Hello @!Current</li>@EndEach</ul></body></html>";
            var model = new List<string>() { "Bob<br/>", "Jim<br/>", "Bill<br/>" };
            var output = Parse(input, model.GetType());

            AssertSyntaxTree(
                output,
                SyntaxTree.WriteString("<html><head></head><body><ul>"),
                SyntaxTree.Iterate(
                    SyntaxTreeExpression.Self(model.GetType()),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("<li>Hello "),
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Self(typeof(string)), true),
                        SyntaxTree.WriteString("</li>")
                    )
                ),
                SyntaxTree.WriteString("</ul></body></html>")
            );
        }

        [Test]
        public void Current_with_explaimation_and_parameters_should_html_encode()
        {
            var input = @"<html><head></head><body><ul>@Each.Users;<li>@!Current.Name;</li>@EndEach;</ul></body></html>";
            var model = new { Users = new[] { new User("Bob"), new User("Jim") } };
            var output = Parse(input, model.GetType());

            AssertSyntaxTree(
                output,
                SyntaxTree.WriteString("<html><head></head><body><ul>"),
                SyntaxTree.Iterate(
                    SyntaxTreeExpression.Property(model.GetType(), "Users"),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("<li>"),
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(typeof(User), "Name"), true),
                        SyntaxTree.WriteString("</li>")
                    )
                ),
                SyntaxTree.WriteString("</ul></body></html>")
            );
        }

        [Test]
        public void Should_parse_basic_partials()
        {
            var input = @"<html><head></head><body>@Partial['testing'];</body></html>";
            var result = Parse(input, typeof(object));

            AssertSyntaxTree(
                result,
                SyntaxTree.WriteString("<html><head></head><body>"),
                SyntaxTree.Include("testing", SyntaxTreeExpression.Self(typeof(object))),
                SyntaxTree.WriteString("</body></html>")
            );
        }

        [Test]
        public void Should_parse_partial_with_specified_model_property()
        {
            var input = @"<html><head></head><body>@Partial['testing', Model.User];</body></html>";
            var model = new { Name = "Jim", User = new { Name = "Bob" } };
            var result = Parse(input, model.GetType());

            AssertSyntaxTree(
                result,
                SyntaxTree.WriteString("<html><head></head><body>"),
                SyntaxTree.Include("testing", SyntaxTreeExpression.Property(model.GetType(), "User", ExpressionScope.RootModel)),
                SyntaxTree.WriteString("</body></html>")
            );
        }

        [Test]
        public void Should_parse_master_page_if_one_specified()
        {
            var input = "@Master['myMaster']\r\n@Section['Header'];\r\nHeader\r\n@EndSection\r\n@Section['Footer']\r\nFooter\r\n@EndSection";
            var output = Parse(input, typeof(object));

            output.ShouldDeepEqual(
                SyntaxTree.Extend("myMaster", new Dictionary<string, SyntaxTreeNode>
                {
                    { "Header", SyntaxTree.Block(SyntaxTree.WriteString("\r\nHeader\r\n")) },
                    { "Footer", SyntaxTree.Block(SyntaxTree.WriteString("\r\nFooter\r\n")) }
                })
            );
        }

        [Test]
        public void Should_parse_sections_in_master_page()
        {
            var input = @"<div id='header'>@Section['Header'];</div><div id='footer'>@Section['Footer'];</div>";
            var result = Parse(input, typeof(object));

            AssertSyntaxTree(
                result,
                SyntaxTree.WriteString("<div id='header'>"),
                SyntaxTree.Override("Header"),
                SyntaxTree.WriteString("</div><div id='footer'>"),
                SyntaxTree.Override("Footer"),
                SyntaxTree.WriteString("</div>")
            );
        }

        [Test]
        public void Should_handle_master_page_hierarchies()
        {
            var input = "@Master['top']\r\n@Section['TopContent']Top\r\n@Section['MiddleContent']@EndSection";
            var result = Parse(input, typeof(object));

            result.ShouldDeepEqual(
                SyntaxTree.Extend("top", new Dictionary<string, SyntaxTreeNode>()
                {
                    {
                        "TopContent",
                        SyntaxTree.Block(
                            SyntaxTree.WriteString("Top\r\n"),
                            SyntaxTree.Override("MiddleContent")
                        )
                    }
                })
            );
        }

        [Test]
        public void Should_parse_flush()
        {
            var input = "Header@Flush;Footer";
            var result = Parse(input, typeof(object));

            AssertSyntaxTree(
                result,
                SyntaxTree.WriteString("Header"),
                SyntaxTree.Flush(),
                SyntaxTree.WriteString("Footer")
            );
        }

        [Test]
        public void Should_include_block_with_ifnull_if_value_null()
        {
            var input = @"<html><head></head><body>@IfNull.Name;No users found@EndIf;</body></html>";
            var output = Parse(input, typeof(User));

            AssertSyntaxTree(
                output,
                SyntaxTree.WriteString("<html><head></head><body>"),
                SyntaxTree.Conditional(SyntaxTreeExpression.Property(typeof(User), "Name"),
                    SyntaxTree.Block(),
                    SyntaxTree.Block(SyntaxTree.WriteString("No users found"))
                ),
                SyntaxTree.WriteString("</body></html>")
            );
        }

        [Test]
        public void Should_include_block_with_ifnotnull_if_value_non_null()
        {
            var input = @"<html><head></head><body>@IfNotNull.Name;Hello @Model.Name@EndIf;</body></html>";
            var output = Parse(input, typeof(User));

            AssertSyntaxTree(
                output,
                SyntaxTree.WriteString("<html><head></head><body>"),
                SyntaxTree.Conditional(SyntaxTreeExpression.Property(typeof(User), "Name"),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("Hello "),
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(typeof(User), "Name", ExpressionScope.RootModel))
                    ),
                    null
                ),
                SyntaxTree.WriteString("</body></html>")
            );
        }

        [TestCase("@Master['Master'];@Section['Middle'];Hello@EndSection;Content out here")]
        [TestCase("@Master['Master'];@Section['Middle'];Hello@EndSection;@Model;")]
        [TestCase("@Master;")]
        [TestCase("@Master[];")]
        [TestCase("@Master[''];")]
        [TestCase("@Master['Master'];@Section")]
        [TestCase("@Master['Master'];@Section[]")]
        [TestCase("@Master['Master'];@Section['']")]
        [TestCase("@Master['Master'];@EndSection;")]
        [TestCase("Foo @EndSection; Bar")]
        [TestCase("Foo @EndIf; Bar")]
        [TestCase("Foo @EndEach; Bar")]
        [TestCase("@Section;")]
        [TestCase("@Section[]")]
        [TestCase("@Section['']")]
        [TestCase("@Partial;")]
        [TestCase("@Partial[]")]
        [TestCase("@Partial['']")]
        [TestCase("@If.;")]
        [TestCase("@If;")]
        [TestCase("@IfNull.;")]
        [TestCase("@IfNull;")]
        [TestCase("@IfNot.;")]
        [TestCase("@IfNot;")]
        [TestCase("@IfNotNull.;")]
        [TestCase("@IfNotNull;")]
        [TestCase("Hello @Each.Items; World")]
        [TestCase("Hello @If.Test; World")]
        public void Should_throw_for_invalid_templates(string template)
        {
            var model = new { Test = false, Items = new string[0] };
            Assert.Throws<VeilParserException>(() =>
            {
                Parse(template, model.GetType());
            });
        }

        /*
                [Test]
                public void Should_call_to_expand_paths()
                {
                    const string input = @"<script src='@Path['~/scripts/test.js']'></script>";
                    var fakeViewEngineHost = new FakeViewEngineHost();
                    fakeViewEngineHost.ExpandPathCallBack = s => s.Replace("~/", "/BasePath/");
                    var viewEngine = new SuperSimpleViewEngine();

                    var result = viewEngine.Render(input, null, fakeViewEngineHost);

                    Assert.Equal("<script src='/BasePath/scripts/test.js'></script>", result);
                }

                [Test]
                public void Should_expand_anti_forgery_tokens()
                {
                    const string input = "<html><body><form>@AntiForgeryToken</form><body></html>";
                    var fakeViewEngineHost = new FakeViewEngineHost();
                    var viewEngine = new SuperSimpleViewEngine();

                    var result = viewEngine.Render(input, null, fakeViewEngineHost);

                    Assert.Equal("<html><body><form>CSRF</form><body></html>", result);
                }
         */
    }

    public class User
    {
        public User(string name)
            : this(name, false)
        {
        }

        public User(string name, bool isFriend)
        {
            Name = name;
            IsFriend = isFriend;
        }

        public string Name { get; private set; }

        public bool IsFriend { get; private set; }
    }

    public class FakeModel
    {
        public FakeModel(string name, List<string> users)
        {
            this.Name = name;
            this.Users = users;
        }

        public List<string> Users { get; private set; }

        public string Name { get; private set; }

        public bool HasUsers
        {
            get
            {
                return this.Users.Any();
            }
        }
    }
}