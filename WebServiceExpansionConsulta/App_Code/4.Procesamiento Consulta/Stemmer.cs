/*************************************************************************
 *                                                                       *
 * Stemmer.cs                                                            *
 *                                                                       *
 *************************************************************************
 *                                                                       *
 * Implementation of the Porter Stemming Alorithm                        *
 *                                                                       *
 * Copyright (c) 2009 Wilfred Wong <wilfred.wong@divergencehosting.com>  *
 * All rights reserved.                                                  *
 *                                                                       *
 * This script is free software; you can redistribute it and/or modify   *
 * it under the terms of the GNU General Public License as published by  *
 * the Free Software Foundation; either version 2 of the License, or     *
 * (at your option) any later version.                                   *
 *                                                                       *
 * The GNU General Public License can be found at                        *
 * http://www.gnu.org/copyleft/gpl.html.                                 *
 *                                                                       *
 * This script is distributed in the hope that it will be useful,        *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the          *
 * GNU General Public License for more details.                          *
 *                                                                       *
 * Author(s): Wilfred Wong <wilfred.wong@divergencehosting.com>          *
 *                                                                       *
 * Last modified: 08/08/07                                               *
 *                                                                       *
 *************************************************************************/

/// <summary>
/**
 *  This is a complete port of the PHP code written by Jon Abernathy <jon@chuggnutt.com> 
 *  It can be found here http://www.chuggnutt.com/stemmer.php
 *  The majority of comments in the file are exact replicas of Jon's comments
 * 
 *  Takes a word, or list of words, and reduces them to their English stems.
 *
 *  This is a fairly faithful implementation of the Porter stemming algorithm that
 *  reduces English words to their stems, originally adapted from the ANSI-C code found
 *  on the official Porter Stemming Algorithm website, located at
 *  http://www.tartarus.org/~martin/PorterStemmer and later changed to conform
 *  more accurately to the algorithm itself.
 *
 *  There is a deviation in the way compound words are stemmed, such as
 *  hyphenated words and words starting with certain prefixes. For instance,
 *  "international" should be reduced to "internation" and not "intern," but
 *  an unmodified version of the alorithm will do just that. Currently, only
 *  hyphenated words are accounted for.
 *
 *  @author Wilfred Wong <wilfred.wong@divergencehosting.com>
 *  @version 1.0
 */
/// </summary>

using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace ModeloSemantico_PU
{
    public class Stemmer
    {
        public string stem(string p_word)
        {
            if (p_word.Length == 0)
            {
                return "";
            }

            string result = "";

            p_word = p_word.ToLower();

            // Strip punctuation, etc. Keep ' and . for URLs and contractions.
            if (p_word.Substring(p_word.Length - 2) == "'s")
            {
                p_word = p_word.Substring(0, p_word.Length - 2);
            }


            p_word = Regex.Replace(p_word, "/[^a-z0-9'.-]/", "");

            string first = "";

            if (p_word.IndexOf("-") > 0)
            {
                first = p_word.Substring(0, p_word.IndexOf("-") + 1); // Grabs hyphen too
                p_word = p_word.Substring(p_word.IndexOf("-") + 1);
            }

            if (p_word.Length > 2)
            {
                p_word = this.Step1(p_word);
                p_word = this.Step2(p_word);
                p_word = this.Step3(p_word);
                p_word = this.Step4(p_word);
                p_word = this.Step5(p_word);
            }

            result = first + p_word;

            return result;
        }

        /**
         *  Takes a list of words and returns them reduced to their stems.
         *
         *  p_words can be either a string or an array. If it is a string, it will
         *  be split into separate words on whitespace, commas, or semicolons. If
         *  an array, it assumes one word per element.
         *
         *  @param mixed p_words String or array of word(s) to reduce
         *  @access public
         *  @return array List of word stems
         */
        public ArrayList stem_list(object p_words)
        {
            string[] newwords;

            ArrayList results = new ArrayList();

            if (p_words is string)
            {
                char[] seperator = { Convert.ToChar(" "), Convert.ToChar(","), Convert.ToChar(";") };
                newwords = p_words.ToString().Split(seperator);

                for (int i = 0; i <= newwords.GetUpperBound(0); i++)
                {
                    results.Add(this.stem(newwords[i]));
                }
            }
            else
            {
                newwords = (string[])p_words;
                foreach (string item in newwords)
                {
                    string result = this.stem(item);

                    if (result != "")
                        results.Add(result);

                }
            }

            return results;
        }

        // Five Step Process to Stem a string
        #region Five Steps to Stem
        /**
     *  Performs the functions of steps 1a and 1b of the Porter Stemming Algorithm.
     *
     *  First, if the word is in plural form, it is reduced to singular form.
     *  Then, any -ed or -ing endings are removed as appropriate, and finally,
     *  words ending in "y" with a vowel in the stem have the "y" changed to "i".
     *
     *  @param string $word Word to reduce
     *  @access private
     *  @return string Reduced word
     */
        public string Step1(string p_word)
        {
            // Step 1a
            if (p_word.Substring(p_word.Length - 1) == "s")
            {
                if (p_word.Substring(p_word.Length - 4) == "sses")
                {
                    p_word = p_word.Substring(0, p_word.Length - 2);
                }
                else if (p_word.Substring(p_word.Length - 3) == "ies")
                {
                    p_word = p_word.Substring(0, p_word.Length - 2);
                }
                else if (p_word.Substring(p_word.Length - 2, 1) != "s")
                {
                    // If second-to-last character is not "s"
                    p_word = p_word.Substring(0, p_word.Length - 1);
                }
            }
            // Step 1b
            if (p_word.Substring(p_word.Length - 3) == "eed")
            {
                if (this.count_vc(p_word.Substring(0, p_word.Length - 3)) > 0)
                {
                    // Convert '-eed' to '-ee'
                    p_word = p_word.Substring(0, p_word.Length - 1);
                }
            }
            else
            {
                if (Regex.IsMatch("/([aeiou]|[^aeiou]y).*(ed|ing)$/", p_word))
                { // vowel in stem
                    // Strip '-ed' or '-ing'
                    if (p_word.Substring(p_word.Length - 2) == "ed")
                    {
                        p_word = p_word.Substring(0, p_word.Length - 2);
                    }
                    else
                    {
                        p_word = p_word.Substring(0, p_word.Length - 3);
                    }
                    if (p_word.Substring(p_word.Length - 2) == "at" || p_word.Substring(p_word.Length - 2) == "bl" ||
                         p_word.Substring(p_word.Length - 2) == "iz")
                    {
                        p_word += "e";
                    }
                    else
                    {
                        string last_char = p_word.Substring(p_word.Length - 1, 1);
                        string next_to_last = p_word.Substring(p_word.Length - 2, 1);
                        // Strip ending double consonants to single, unless "l", "s" or "z"
                        if (this.is_consonant(p_word, -1) &&
                             last_char == next_to_last &&
                             last_char != "l" && last_char != "s" && last_char != "z")
                        {
                            p_word = p_word.Substring(0, p_word.Length - 1);
                        }
                        else
                        {
                            // If VC, and cvc (but not w,x,y at end)
                            if (this.count_vc(p_word) == 1 && this._o(p_word))
                            {
                                p_word += "e";
                            }
                        }
                    }
                }
            }
            // Step 1c
            // Turn y into i when another vowel in stem
            if (Regex.IsMatch("/([aeiou]|[^aeiou]y).*y$/", p_word))
            { // vowel in stem
                p_word = p_word.Substring(0, p_word.Length - 1) + "i";
            }
            return p_word;
        }

        /**
         *  Performs the function of step 2 of the Porter Stemming Algorithm.
         *
         *  Step 2 maps double suffixes to single ones when the second-to-last character
         *  matches the given letters. So "-ization" (which is "-ize" plus "-ation"
         *  becomes "-ize". Mapping to a single character occurence speeds up the script
         *  by reducing the number of possible string searches.
         *
         *  Note: for this step (and steps 3 and 4), the algorithm requires that if
         *  a suffix match is found (checks longest first), then the step ends, regardless
         *  if a replacement occurred. Some (or many) implementations simply keep
         *  searching though a list of suffixes, even if one is found.
         *
         *  @param string $word Word to reduce
         *  @access private
         *  @return string Reduced word
         */
        public string Step2(string p_word)
        {
            switch (p_word.Substring(p_word.Length - 2, 1))
            {
                case "a":
                    if (this.ReplaceSuffix(ref p_word, "ational", "ate", 0))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "tional", "tion", 0))
                    {
                        return p_word;
                    }
                    break;
                case "c":
                    if (this.ReplaceSuffix(ref p_word, "enci", "ence", 0))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "anci", "ance", 0))
                    {
                        return p_word;
                    }
                    break;
                case "e":
                    if (this.ReplaceSuffix(ref p_word, "izer", "ize", 0))
                    {
                        return p_word;
                    }
                    break;
                case "l":
                    // This condition is a departure from the original algorithm;
                    // I adapted it from the departure in the ANSI-C version.
                    if (this.ReplaceSuffix(ref p_word, "bli", "ble", 0))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "alli", "al", 0))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "entli", "ent", 0))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "eli", "e", 0))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "ousli", "ous", 0))
                    {
                        return p_word;
                    }
                    break;
                case "o":
                    if (this.ReplaceSuffix(ref p_word, "ization", "ize", 0))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "isation", "ize", 0))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "ation", "ate", 0))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "ator", "ate", 0))
                    {
                        return p_word;
                    }
                    break;
                case "s":
                    if (this.ReplaceSuffix(ref p_word, "alism", "al", 0))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "iveness", "ive", 0))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "fulness", "ful", 0))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "ousness", "ous", 0))
                    {
                        return p_word;
                    }
                    break;
                case "t":
                    if (this.ReplaceSuffix(ref p_word, "aliti", "al", 0))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "iviti", "ive", 0))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "biliti", "ble", 0))
                    {
                        return p_word;
                    }
                    break;
                case "g":
                    // This condition is a departure from the original algorithm;
                    // I adapted it from the departure in the ANSI-C version.
                    if (this.ReplaceSuffix(ref p_word, "logi", "log", 0))
                    { //*****
                        return p_word;
                    }
                    break;
            }
            return p_word;
        }

        /**
         *  Performs the function of step 3 of the Porter Stemming Algorithm.
         *
         *  Step 3 works in a similar stragegy to step 2, though checking the
         *  last character.
         *
         *  @param string $word Word to reduce
         *  @access private
         *  @return string Reduced word
         */
        public string Step3(string p_word)
        {
            switch (p_word.Substring(p_word.Length - 1))
            {
                case "e":
                    if (this.ReplaceSuffix(ref p_word, "icate", "ic", 0))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "ative", "", 0))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "alize", "al", 0))
                    {
                        return p_word;
                    }
                    break;
                case "i":
                    if (this.ReplaceSuffix(ref p_word, "iciti", "ic", 0))
                    {
                        return p_word;
                    }
                    break;
                case "l":
                    if (this.ReplaceSuffix(ref p_word, "ical", "ic", 0))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "ful", "", 0))
                    {
                        return p_word;
                    }
                    break;
                case "s":
                    if (this.ReplaceSuffix(ref p_word, "ness", "", 0))
                    {
                        return p_word;
                    }
                    break;
            }
            return p_word;
        }

        /**
         *  Performs the function of step 4 of the Porter Stemming Algorithm.
         *
         *  Step 4 works similarly to steps 3 and 2, above, though it removes
         *  the endings in the context of VCVC (vowel-consonant-vowel-consonant
         *  combinations).
         *
         *  @param string $word Word to reduce
         *  @access private
         *  @return string Reduced word
         */
        public string Step4(string p_word)
        {
            switch (p_word.Substring(p_word.Length - 2, 1))
            {
                case "a":
                    if (this.ReplaceSuffix(ref p_word, "al", "", 1))
                    {
                        return p_word;
                    }
                    break;
                case "c":
                    if (this.ReplaceSuffix(ref p_word, "ance", "", 1))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "ence", "", 1))
                    {
                        return p_word;
                    }
                    break;
                case "e":
                    if (this.ReplaceSuffix(ref p_word, "er", "", 1))
                    {
                        return p_word;
                    }
                    break;
                case "i":
                    if (this.ReplaceSuffix(ref p_word, "ic", "", 1))
                    {
                        return p_word;
                    }
                    break;
                case "l":
                    if (this.ReplaceSuffix(ref p_word, "able", "", 1))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "ible", "", 1))
                    {
                        return p_word;
                    }
                    break;
                case "n":
                    if (this.ReplaceSuffix(ref p_word, "ant", "", 1))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "ement", "", 1))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "ment", "", 1))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "ent", "", 1))
                    {
                        return p_word;
                    }
                    break;
                case "o":
                    // special cases
                    if (p_word.Substring(p_word.Length - 4) == "sion" || p_word.Substring(p_word.Length - 4) == "tion" || p_word.Substring(p_word.Length - 3) == "ing")
                    {
                        if (this.ReplaceSuffix(ref p_word, "ion", "", 1))
                        {
                            return p_word;
                        }

                        if (this.ReplaceSuffix(ref p_word, "ng", "", 1))
                        {
                            return p_word;
                        }


                    }
                    if (this.ReplaceSuffix(ref p_word, "ou", "", 1))
                    {
                        return p_word;
                    }
                    break;
                case "s":
                    if (this.ReplaceSuffix(ref p_word, "ism", "", 1))
                    {
                        return p_word;
                    }
                    break;
                case "t":
                    if (this.ReplaceSuffix(ref p_word, "ate", "", 1))
                    {
                        return p_word;
                    }
                    if (this.ReplaceSuffix(ref p_word, "iti", "", 1))
                    {
                        return p_word;
                    }
                    break;
                case "u":
                    if (this.ReplaceSuffix(ref p_word, "ous", "", 1))
                    {
                        return p_word;
                    }
                    break;
                case "v":
                    if (this.ReplaceSuffix(ref p_word, "ive", "", 1))
                    {
                        return p_word;
                    }
                    break;
                case "z":
                    if (this.ReplaceSuffix(ref p_word, "ize", "", 1))
                    {
                        return p_word;
                    }
                    break;
            }
            return p_word;
        }

        /**
         *  Performs the function of step 5 of the Porter Stemming Algorithm.
         *
         *  Step 5 removes a final "-e" and changes "-ll" to "-l" in the context
         *  of VCVC (vowel-consonant-vowel-consonant combinations).
         *
         *  @param string $word Word to reduce
         *  @access private
         *  @return string Reduced word
         */
        private string Step5(string p_word)
        {
            if (p_word.Substring(p_word.Length - 1) == "e")
            {
                string shortword = p_word.Substring(0, p_word.Length - 1);
                // Only remove in vcvc context...
                if (this.count_vc(shortword) > 1)
                {
                    p_word = shortword;
                }
                else if (this.count_vc(shortword) == 1 && !this._o(shortword))
                {
                    p_word = shortword;
                }
            }
            if (p_word.Substring(p_word.Length - 2) == "ll")
            {
                // Only remove in vcvc context...
                if (this.count_vc(p_word) > 1)
                {
                    p_word = p_word.Substring(0, p_word.Length - 1);
                }
            }
            return p_word;
        }

        #endregion

        #region Helper Functions
        /**
     *  Checks that the specified letter (position) in the word is a consonant.
     *
     *  Handy check adapted from the ANSI C program. Regular vowels always return
     *  FALSE, while "y" is a special case: if the prececing character is a vowel,
     *  "y" is a consonant, otherwise it's a vowel.
     *
     *  And, if checking "y" in the first position and the word starts with "yy",
     *  return true even though it's not a legitimate word (it crashes otherwise).
     *
     *  @param string $word Word to check
     *  @param integer $pos Position in the string to check
     *  @access public
     *  @return boolean
     */
        private bool is_consonant(string p_word, int p_pos)
        {
            // Sanity checking $pos
            if (Math.Abs(p_pos) > p_word.Length)
            {
                if (p_pos < 0)
                {
                    // Points "too far back" in the string. Set it to beginning.
                    p_pos = 0;
                }
                else
                {
                    // Points "too far forward." Set it to end.
                    p_pos = -1;
                }
            }
            string charpos = p_word.Substring(p_pos, 1);
            switch (charpos)
            {
                case "a":
                case "e":
                case "i":
                case "o":
                case "u":
                    return false;
                case "y":
                    if (p_pos == 0 || p_word.Length == -p_pos)
                    {
                        // Check second letter of word.
                        // If word starts with "yy", return true.
                        if (p_word.Substring(1, 1) == "y")
                        {
                            return true;
                        }
                        return !(this.is_consonant(p_word, 1));
                    }
                    else
                    {
                        return !(this.is_consonant(p_word, p_pos - 1));
                    }
                default:
                    return true;
            }
        }

        /**
         *  Counts (measures) the number of vowel-consonant occurences.
         *
         *  Based on the algorithm; this handy function counts the number of
         *  occurences of vowels (1 or more) followed by consonants (1 or more),
         *  ignoring any beginning consonants or trailing vowels. A legitimate
         *  VC combination counts as 1 (ie. VCVC = 2, VCVCVC = 3, etc.).
         *
         *  @param string $word Word to measure
         *  @access public
         *  @return integer
         */
        private double count_vc(string p_word)
        {
            double m = 0;
            int length = p_word.Length;
            bool prev_c = false;
            for (int i = 0; i < length; i++)
            {
                bool is_c = this.is_consonant(p_word, i);
                if (is_c)
                {
                    if (m > 0 && !prev_c)
                    {
                        m += 0.5;
                    }
                }
                else
                {
                    if (prev_c || m == 0)
                    {
                        m += 0.5;
                    }
                }
                prev_c = is_c;
            }
            m = Math.Floor(m);
            return m;
        }

        /**
         *  Checks for a specific consonant-vowel-consonant condition.
         *
         *  This function is named directly from the original algorithm. It
         *  looks the last three characters of the word ending as
         *  consonant-vowel-consonant, with the final consonant NOT being one
         *  of "w", "x" or "y".
         *
         *  @param string $word Word to check
         *  @access private
         *  @return boolean
         */
        private bool _o(string p_word)
        {
            if (p_word.Length >= 3)
            {
                if (this.is_consonant(p_word, -1) && !this.is_consonant(p_word, -2) &&
                     this.is_consonant(p_word, -3))
                {
                    string last_char = p_word.Substring(p_word.Length - 1);
                    if (last_char == "w" || last_char == "x" || last_char == "y")
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }

        /**
         *  Replaces suffix, if found and word measure is a minimum count.
         *
         *  @param string $word Word to check and modify
         *  @param string $suffix Suffix to look for
         *  @param string $replace Suffix replacement
         *  @param integer $m Word measure value that the word must be greater
         *                    than to replace
         *  @access private
         *  @return boolean
         */
        private bool ReplaceSuffix(ref string p_word, string p_suffix, string p_replace, int m)
        {
            int sl = p_suffix.Length;
            // added to handle case when suffix is larger than the word
            if (p_word.Length >= sl)
            {
                // if (p_word.Substring(p_word.Length - sl) == p_suffix)
                //  {
                // substr_replace($word, '', -$sl)
                string shortword = p_word.Substring(0, p_word.Length - sl);
                p_word = shortword;
                //if (this.count_vc(shortword) > m)
                //{
                //    p_word = shortword + p_replace;
                //}
                // Found this suffix, doesn't matter if replacement succeeded
                return true;
                //  }
            }
            return false;
        }
        #endregion
    }
}
