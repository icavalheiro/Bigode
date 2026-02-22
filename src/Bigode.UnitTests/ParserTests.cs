using Bigode.Models;

namespace Bigode.UnitTests;

public class ParserTests
{
    private readonly Bigode bigode = new(".html");

    [Test]
    public async Task Should_ReplaceSimpleVar_NoSpace()
    {
        var templatePath = await Tools.WriteTempTemplate("basic", "Hello {{name}}!");
        var result = await bigode.Parse(templatePath, new RenderModel
        {
            {"name", new ("World")}
        });

        await Assert.That(result).IsEqualTo("Hello World!");
    }

    [Test]
    public async Task Should_ReplaceSimpleVar_WithSpace()
    {
        var templatePath = await Tools.WriteTempTemplate("basic2", "Hello {{ name }}!");
        var result = await bigode.Parse(templatePath, new RenderModel
        {
            {"name", new ("World")}
        });

        await Assert.That(result).IsEqualTo("Hello World!");
    }

    [Test]
    public async Task Should_IgnoreComments()
    {
        var templatePath = await Tools.WriteTempTemplate("ignore", "Visible {{! This is hidden }} Content");
        var result = await bigode.Parse(templatePath, []);

        await Assert.That(result).IsEqualTo("Visible  Content");
    }

    [Test]
    public async Task Should_RenderLambdas()
    {
        var templatePath = await Tools.WriteTempTemplate("lambda", "{{#wrapped}}{{name}} is awesome.{{/wrapped}}");
        var result = await bigode.Parse(templatePath, new RenderModel
        {
            {"name", new ("Lambda")},
            {"wrapped", new(async (content) =>
            {
                return $"<b>{content}</b>";
            })}
        });

        await Assert.That(result).IsEqualTo("<b>Lambda is awesome.</b>");
    }

    [Test]
    public async Task Should_RenderLoopSections()
    {
        var templatePath = await Tools.WriteTempTemplate("loop", "Users: {{#users}}{{name}}, {{/users}}");
        var result = await bigode.Parse(templatePath, new RenderModel
        {
            {"users", new ([
                new RenderModel
                {
                    {"name", new ("Alice")},
                },
                new RenderModel
                {
                    {"name", new ("Bob")},
                }
            ])}
        });

        await Assert.That(result).IsEqualTo("Users: Alice, Bob, ");
    }

    [Test]
    public async Task Should_RenderConditionalSections()
    {
        var templatePath = await Tools.WriteTempTemplate("conditional",
            "{{#isAdmin}}Welcome Admin{{/isAdmin}}{{#isGuest}}Welcome Guest{{/isGuest}}"
        );

        var result = await bigode.Parse(templatePath, new RenderModel
        {
            {"isAdmin", new (true)},
            {"isGuest", new (false)},
        });

        await Assert.That(result).IsEqualTo("Welcome Admin");
    }

    [Test]
    public async Task Should_RenderInvertedSections()
    {
        var templatePath = await Tools.WriteTempTemplate("inverted",
            "{{^data}}No Data Found{{/data}}"
        );

        var result = await bigode.Parse(templatePath, new RenderModel
        {
            {"data", new (false)},
        });

        await Assert.That(result).IsEqualTo("No Data Found");
    }

    [Test]
    public async Task Should_RenderNestedSections()
    {
        var templatePath = await Tools.WriteTempTemplate("nested",
            "{{#categories}}- {{name}}: {{#items}}{{name}}, {{/items}}{{/categories}}"
        );

        var result = await bigode.Parse(templatePath, new RenderModel
        {
            {"categories", new([
                new RenderModel {
                    {"name", new ("Fruit")},
                    {"items", new ([
                        new RenderModel{
                            {"name", new ("Apple")},
                        },
                        new RenderModel{
                            {"name", new ("Banana")},
                        },
                    ])}
                },
                new RenderModel {
                    {"name", new ("Tech")},
                    {"items", new ([
                        new RenderModel{
                            {"name", new ("Bun")},
                        },
                        new RenderModel{
                            {"name", new ("TS")},
                        },
                    ])}
                },
            ])}
        });

        await Assert.That(result).IsEqualTo("- Fruit: Apple, Banana, - Tech: Bun, TS, ");
    }

    [Test]
    public async Task Should_RenderEmptyOnMissingVarSections()
    {
        var templatePath = await Tools.WriteTempTemplate("missing", "Nothing here: [{{ missing }}]");

        var result = await bigode.Parse(templatePath, []);

        await Assert.That(result).IsEqualTo("Nothing here: []");
    }

    [Test]
    public async Task Should_ThrowOnMismatchTags()
    {
        var templatePath = await Tools.WriteTempTemplate("mismatch", "{{#open}} ... {{/close}}");

        var observed = await Assert.ThrowsAsync(async () =>
        {
            await bigode.Parse(templatePath, []);
        });

        await Assert.That(observed).HasMessageContaining("Mismatched section");
    }

    [Test]
    public async Task Should_ThrowOnUnclosedTags()
    {
        var templatePath = await Tools.WriteTempTemplate("unclosed", "{{#open}} ... ");

        var observed = await Assert.ThrowsAsync(async () =>
        {
            await bigode.Parse(templatePath, []);
        });

        await Assert.That(observed).HasMessageContaining("Unclosed section");
    }

    [Test]
    public async Task Should_RenderPartials()
    {
        await Tools.WriteTempTemplate("user_card", "User: {{ name }}");
        var templatePath = await Tools.WriteTempTemplate("main_template", "<div>{{> user_card }}</div>");
        var result = await bigode.Parse(templatePath, new RenderModel
        {
            {"name", new ("Alice")}
        });

        await Assert.That(result).IsEqualTo("<div>User: Alice</div>");
    }

    [Test]
    public async Task Should_RenderLoopPartials()
    {
        await Tools.WriteTempTemplate("item", "<li>{{ name }}</li>");
        var templatePath = await Tools.WriteTempTemplate("list_template", "<ul>{{#items}}{{> item }}{{/items}}</ul>");
        var result = await bigode.Parse(templatePath, new RenderModel
        {
            {"items", new ([
                new RenderModel {
                    {"name", new("Apples")},
                },
                new RenderModel {
                    {"name", new("Bananas")},
                },
            ])}
        });

        await Assert.That(result).IsEqualTo("<ul><li>Apples</li><li>Bananas</li></ul>");
    }

    [Test]
    public async Task Should_RenderNestedPartials()
    {
        await Tools.WriteTempTemplate("leaf", "I am the {{name}}.");
        await Tools.WriteTempTemplate("branch", "Branch including: {{> leaf }}");
        var templatePath = await Tools.WriteTempTemplate("root", "Root starts: {{> branch }}");
        var result = await bigode.Parse(templatePath, new RenderModel
        {
            {"name", new ("leaf")}
        });

        await Assert.That(result).IsEqualTo("Root starts: Branch including: I am the leaf.");
    }
}