# BibleTaggingUtility
 Add and/or correct Strong's tags
 
In order to use this utility, you need to have a bibles folder in your system, which contains a folder for each bible to be tagged.<br>
<br>
For Example<br><br>
&nbsp;&nbsp;&nbsp;&nbsp;Bibles (folder)<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;+---MyBible (folder)<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;+--- tagged (folder)<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;+---BibleConfig.txt (file)<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;+---MyOtherBible (folder)<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;+--- tagged (folder)<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;+---BibleConfig.txt (file)<br>
<br>
Each Bible folder should contain a configuration file BiblesConfig.txt and a subfolder named tagged.
The tagged subfolder should contain only one file which is the version of the tagged bible to be edited or corrected in a verse per line text file format (see note) as in the following example:<br>
Gen 1:13 And the evening <06153> and the morning <01242> were the third <07992> day <03117>. <br>
================== <br>
Note: For release 5 and above, you can use an already tagged OSIS xml file for the purpose of modifying the tag values.<br>
================== <br>
<br>
Refer to [**BibleTaggingPreperation**](https://github.com/sabdelmalik/BibleTaggingPreperation) for generating the initial tagged file.<br>
<br>

Following is a sample BiblesConfig.txt (for release 5, when using an OSIS XML file, set osis=true, and the OSIS section below needs the "osisIDWork" and the "output-file" entries only)

[Tagging]<br>
taggedBible=mybible.txt<br>
osis=false
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

# Utility short-cut keys
Ctrl-S Save<br>
Ctrl-Y Redo<br>
Ctrl-Z Undo<br>
Pg dn Next verse<br>
Pg up Previous verse<br>

## Drag and Drop behaviour
When a tag is dragged from a reference verse (top two tables) to the target verse (bottom table), the dragged tag will replace the existing tag(s).<br>
However, if the dragged tag is dropped while the Ctrl key is pressed, it is added to the existing tag(s).

## Merging two or more target words
(not available for osis xml)<br>
Highlight the target words, then right click on one of them. Click on Merge from the context menu.
The merged cells must be adjacent words, otherwise you will not see the Merge menu.
to higlight adjacent words, click on the first word, and while the mouse is down, dag it to the right or left as required.
## Splitting a multi word cell
(not available for osis xml)<br>
If a target word cell contains more than word, it can be split into seperate words. Click the word cell to select it. Right click the cell and click Split from the contxt menu.
## Swapping two tags
Select the two words that their tags to be swapped. Right click one of them and click the cell and click Swap from the contxt menu.
To select two words, click on the first word to select it. While pressing Ctrl, click the second word.
## Deleteing tag value
Right click the tag to delete using the context menu
