##PERLRUNNER!!!1!! Use at YOUR OWN RISK.

![Screenshot of PERLRUNNER!!!1!!](https://raw.githubusercontent.com/ruffin--/PerlRunner/master/screenshots/runner1.png)

(Imagine some neat audio effects going off while you're reading the name, above. And yes, I know there's a "Perl Runner" for Mac that'll set you back **$1.99**. But, one, the name's not copyrighted. Two, there's no space in PERLRUNNER!!!1!.)

###License

PerlRunner is [MPL 2.0 licensed](https://www.mozilla.org/MPL/2.0/). See included LICENSE file for the license in its entirety.

###Why?

Feeling a little like a Connecticut Yankee in King Arthur's Court, I unfortunately stumbled into a project at work that required Perl maintenance in 2015. And I noticed that, except for Padre, there wasn't an obvious, free Perl IDE, and [Padre doesn't seem to like Strawberry right now](http://myfreakinname.blogspot.com/2015/01/continued-misadventures-in-perl.html#padrefail).

And editing in one window, alt-tabbing, cursor up-ing, return-ing, and alt-tabbing back to fix your bugs gets old *really* quickly. How in the world is it that [Open Perl IDE](http://open-perl-ide.sourceforge.net/) is the closest we've got to a free, native, easy to install dev environ for Perl on Windows, and *it no longer works at all*? I'm worried about Jürgen Güntherodt, honestly. Hope he's okay.

So I wanted to F5 my way through learning Perl, and took an afternoon to learn and hack a little WPF and XAML. Keep your [Tk craziness](http://ptkdb.sourceforge.net/demo.html) away from me.

###Just let me F5.

Check. That's really all PerlRunner does.

1. Open a .pl file.
2. Hit F5.
3. See the results.
4. Profit.

You're welcome. ;^)

###Features

* It automatically finds which Perl install you have in your path and uses that. `where perl` indeed.
* You can open *many* file*s*, and *save your changes*. Wow. I *know*.
* You can run `perl` with the `-l` option on to make `print`s look better. (on by default)
* Tabs close! Hit `ctrl-w` and be boggled.
* Did I mention you can hit F5 and have it execute?

Yeah, that's about it for features right now.

**Note:** You can set the `-l` option with the `Execute >>> Use -l option` menu. It is *on* by default, and resets to on each time PerlRunner is opened. 

###It's not an IDE. kk?

Right now all it does is open files (that don't share the same file name), no syntax highlighting, and no options.

Stuff I'll probably add if I stay stuck in Perlland.

1. Stuff any self-respecting app *should* do.
    * Create and save a new file (right now you *have* to edit an existing file).
    * Remember...
	    * ... last window size.
    	* ... splitter bar position.
    	* ... last active dir.
    	* ... your cursor position.
    	* ... your birthday.
2. Syntax highlighting (don't hold your breath, but [this project could have potential](https://github.com/PavelTorgashov/FastColoredTextBox) if it's a drop-in replacement, though I'd have to write a Perl syntax parser for it).
3. Line numbers.
4. And if I really get mired in Perl, **DEBUG MODE** (more audio effects).
5. Add vi mode. (Ha. Hahaha. HAHAHAHAHA! No, that's not going to happen. Though I wish it would.)

Adding debugging [really] isn't [as] insane [as it sounds]. Perl has its own debugger. [This from LinuxJournal](http://www.linuxjournal.com/article/2484) is supposed to be a decent intro. I think you could manage marshaling text to and from the debugger running on the command line reasonably easily (ie, straightforwardly).

None of those things are likely. If you're lucky, I'll remove the colors on the text boxes I was using to test my XAML. It's my first WPF app! Woohoo! Channel9, here I come!!

###Enjoy.

Honestly, I know there has to be something somewhere on Windows that'll do this. I can't believe I couldn't google it up in ten seconds or less, but could google up ten different ways to run Perl in a GUI on \*nix. It's *Perl*, for goodness sake. I've been hearing about people using this "language" for 20 years, and I'm still reasonably young. Does nobody do -- or even learn -- Perl on Windows?

Anything doesn't work? *Open an issue!*

###Release notes

<pre>0.1.3 -- Added -W option to Execute menu (on by default) to add warnings to output window after standard return is inserted.
         Added scrollbars to editor and results window.
0.1.2 -- Added -l option to Execute menu (on by default) to add newlines to `print` statements.
         Multiple files can be opened in tabs.
         Icon added.
         Tab characters entered in editor changed to [four] spaces.
         Bug fixes. 
0.1.1 -- Essentially works.
1.0.0 -- Alpha version. Someone forgot to set the version in the assembly information.
</pre>