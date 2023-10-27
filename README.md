# BibleTaggingUtility
 Add and/or correct Strong's tags
 
In order to use this utility, you need to create a bible folder on your system e.g. MyBible.
The folder should contain a configuration file BiblesConfig.txt and a subfolder named tagged
The tagged subfolder should contain only one file which is the version of the tagged bible to be edited or corrected in a verse per line text file format as in the following example:<br>
Gen 1:13 And the evening <06153> and the morning <01242> were the third <07992> day <03117>. <br>
<br>
Refer to [**BibleTaggingPreperation**](https://github.com/sabdelmalik/BibleTaggingPreperation) for generating the initial tagged file.<br>
<br>

Following is a sample BiblesConfig.txt

[Tagging]<br>
taggedBible=mybible.txt<br>
<br>
[OSIS]<br>
osisIDWork=mybible<br>
osisRefWork=bible<br>
language=en<br>
language-type=IETF<br>
title=My Bible(Tagged)<br>
contributor-role=ctb<br>
contributor-name= Simon Peter<br>
type=Bible<br>
identifier=AraSVD-Tagged<br>
description=This work adds Strong's references to ...<br>
rights= ???<br>
refSystem=Bible<br>
ot-vpl-file=ot_tagged_text.txt<br>
nt-vpl-file=nt_tagged_text.txt<br>
output-file=mybible.xml<br>

#Utility short-cut keys
Ctrl-S Save<br>
Ctrl-Y Redo
Ctrl-Z Undo
Pg dn Next verse
Pg up Previous verse

## Drag and Drop behaviour
When a tag is dragged from a reference verse (top two tables) to the target verse (bottom table), the dragged tag will replace the existing tag(s).<br>
However, if the dragged tag is dropped while the Ctrl key is pressed, it is added to the existing tag(s).

## Merging two or more target words
Highlight the target words, then right click on one of them. Click on Merge from the context menu.
The merged cells must be adjacent words, otherwise you will not see the Merge menu.
to higlight adjacent words, click on the first word, and while the mouse is down, dag it to the right or left as required.
## Splitting a multi word cell
If a target word cell contains more than word, it can be split into seperate words. Click the word cell to select it. Right click the cell and click Split from the contxt menu.
## swapping two tags
Select the two words that their tags to be swapped. Right click one of them and click the cell and click Swap from the contxt menu.
To select two words, click on the first word to select it. While pressing Ctrl, click the second word.
