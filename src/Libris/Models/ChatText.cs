using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace Libris.Models
{
    /// <summary>
    ///     A data class representing a text object within the Minecraft client.
    /// </summary>
    public class ChatText
    {
        /// <summary>
        ///     Initializes a new <see cref="ChatText"/> object with no text or styles.
        /// </summary>
        public ChatText()
        {

        }
        /// <summary>
        ///     Initializes a new <see cref="ChatText"/> object with the provided text.
        /// </summary>
        /// <param name="text">The text for the object.</param>
        public ChatText(string text)
        {
            Text = text;
        }

        /// <summary>
        ///     The text of this component.
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }

        /// <summary>
        ///     Whether the text of this component should appear emboldened.
        /// </summary>
        [JsonPropertyName("bold")]
        public bool IsBold { get; set; }

        /// <summary>
        ///     Whether the text of this component should appear italicised.
        /// </summary>
        [JsonPropertyName("italic")]
        public bool IsItalic { get; set; }

        /// <summary>
        ///     Whether the text of this component should appear underlined.
        /// </summary>
        [JsonPropertyName("underlined")]
        public bool IsUnderlined { get; set; }
        
        /// <summary>
        ///     Whether the text of this component should appear with a strike running through it.
        /// </summary>
        [JsonPropertyName("strikethrough")]
        public bool IsStrikethrough { get; set; }

        /// <summary>
        ///     Whether the text of this component should appear obfuscated.
        /// </summary>
        [JsonPropertyName("obfuscated")]
        public bool IsObfuscated { get; set; }

        /// <summary>
        ///     The colour of the text of this component. It is highly recommended to use the <see cref="MinecraftColour"/> class instead of a direct <see cref="string"/>.
        /// </summary>
        [JsonPropertyName("color"/*, ItemConverterType = typeof(MinecraftColourTypeConverter)*/)]
        public string Colour { get; set; }

        /// <summary>
        ///     The subcomponents of this component. Subcomponents appear directly after the parent component.
        /// </summary>
        [JsonPropertyName("extra")]
        public List<ChatText> Subcomponents { get; set; } // sending an empty array is invalid data according to the client ??

        /// <summary>
        ///     Adds a subcomponent to this component's children. Subcomponents appear directly after the parent component.
        /// </summary>
        /// <param name="extra">The subcomponent to add.</param>
        /// <returns>This object.</returns>
        public ChatText AddSubcomponent(ChatText extra)
        {
            if (Subcomponents == null) Subcomponents = new List<ChatText>();
            Subcomponents.Add(extra);
            return this;
        }

        public ChatText SetBold(bool value)
        {
            IsBold = value;
            return this;
        }

        public ChatText SetItalic(bool value)
        {
            IsItalic = value;
            return this;
        }

        public ChatText SetStrikethrough(bool value)
        {
            IsStrikethrough = value;
            return this;
        }

        public ChatText SetObfsucated(bool value)
        {
            IsObfuscated = value;
            return this;
        }

        public ChatText SetUnderline(bool value)
        {
            IsUnderlined = value;
            return this;
        }

        public ChatText SetColour(string value)
        {
            Colour = value;
            return this;
        }
    }
}
