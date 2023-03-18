using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class ExtensionMethods
{
    public static string WrapText(this string text, int width)
    {
        string[] words = text.Split(' ');
        StringBuilder sb = new StringBuilder();
        int lineLength = 0;

        for (int i = 0; i < words.Length; i++)
        {
            string word = words[i];
            int wordLength = word.Length;

            // Check if the word can fit on the current line
            if (lineLength + wordLength <= width)
            {
                sb.Append(word + " ");
                lineLength += wordLength + 1;
            }
            else
            {
                // Check if the word can fit on a new line
                if (wordLength <= width)
                {
                    sb.Append(Environment.NewLine);
                    sb.Append(word + " ");
                    lineLength = wordLength + 1;
                }
                else
                {
                    // The word is longer than the line width, so break it up
                    while (word.Length > 0)
                    {
                        if (lineLength >= width)
                        {
                            sb.Append(Environment.NewLine);
                            lineLength = 0;
                        }
                        int charsToCopy = Math.Min(width - lineLength, word.Length);
                        sb.Append(word.Substring(0, charsToCopy));
                        word = word.Substring(charsToCopy);
                        lineLength += charsToCopy;
                    }
                    sb.Append(" ");
                    lineLength++;
                }
            }
        }

        return sb.ToString().TrimEnd();
    }

    public static Color GetUnityEngineColor(this Colors cardColor)
    {
        string colorName = cardColor.ToString().ToLower();
        Debug.Log(colorName);
        PropertyInfo propertyInfo = typeof(Color).GetProperty(colorName, BindingFlags.Static | BindingFlags.Public);

        if (propertyInfo != null)
        {
            Color color = (Color)propertyInfo.GetValue(null);
            return color;
        }
        else
        {
            Debug.LogError("Color not found: " + colorName);
            return default(Color);
        }
    }
}
