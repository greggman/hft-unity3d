#!/usr/bin/python
import glob
import gzip
import os
import platform
import re
import sh
import shutil
import subprocess
import sys
import time
from optparse import OptionParser

log = lambda *a: None

def VerbosePrint(*args):
    # Print each argument separately so caller doesn't need to
    # stuff everything to be printed into a single string
    for arg in args:
       print arg,
    print


class ShError(Exception):
  def __init__(self, value):
    self.value = value
  def __str__(self):
    return repr(self.value)


def BuildUnityPlugin():
  p = platform.system().lower()
  mdtool_path = "mdtool" if not p == 'darwin' else "/Applications/Unity/MonoDevelop.app/Contents/MacOS/mdtool"
  cmd = sh.Command(mdtool_path)
  result = cmd.build("HappyFunTimes.sln")
  if result.exit_code:
    raise ShError(result)
  log(result)


def CopyFiles(files):
  for file in files:
    src = file["src"]
    dst = file["dst"]
    log("copy", src, "->", dst)


def main(argv):
  """This is the main function."""
  parser = OptionParser()
  parser.add_option(
      "-v", "--verbose", action="store_true",
      help="verbose")

  (options, args) = parser.parse_args(args=argv)

  if options.verbose:
    global log
    log = VerbosePrint

  files = [
    { "src": 'HappyFunTimes/bin/Release/DeJson.dll',
      "dst": 'HFTPlugin/Assets/Plugins/DeJson.dll',
    },
    { "src": 'HappyFunTimes/bin/Release/HappyFunTimes.dll',
      "dst": 'HFTPlugin/Assets/Plugins/HappyFunTimes.dll',
    },
    { "src": 'HappyFunTimes/bin/Release/websocket-sharp.dll',
      "dst": 'HFTPlugin/Assets/Plugins/websocket-sharp.dll',
    },
    { "src": 'Extra/HFTRunner.cs',
      "dst": 'HFTPlugin/Assets/Plugins/HFTExtra.dll',
    },
    { "src": 'HappyFunTimeEditor/bin/Release/HappyFunTimesEditor.dll',
      "dst": 'HFTPlugin/Assets/Plugins/Editor/HappyFunTimesEditor.dll',
    }
  ]


  BuildUnityPlugin()
  CopyFiles(files)

if __name__=="__main__":
  main(sys.argv[1:])




