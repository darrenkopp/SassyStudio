# Release Notes

## 0.6

- New parsing engine which allows for more accurate syntax highlighting
- New experimental intellisense code completion for keywords, variables, functions, and mixins
 - Must enable manually by going to Tools > Options > Sassy Studio > Enable Experimental Intellisense
- Option to replace generated .css files with exception text when there is an error compiling document

## 0.5

- Support for Visual Studio 2013
- Fix overzealous warnings on ==
- Support multiline comment formatting somewhat

By default, CSS files will be generated on save, added to the project nested under the .scss file if
the css file was not already part of the project, and it will be configured as Content. You can
disable each aspect individually, though some aspects don't matter if others are disabled.

## 0.4

- Support generating css file on save (thanks to [NSass project](https://github.com/TBAPI-0KA/NSass))
- Support including generated css in project automatically
- Add options controlling css generation and project integration
- Better color matching

## 0.3

- Support for Comment / Uncomment selection (<kbd>ctrl</kbd> + <kbd>K</kbd>, <kbd>C</kbd> / <kbd>U</kbd>)
- Warnings for deprecated functionality

## 0.2

- Match Existing Color Settings

## 0.1

- Basic Syntax Highlighting Support
- Basic Outlining Support 