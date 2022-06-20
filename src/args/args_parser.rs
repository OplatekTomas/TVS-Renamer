use std::fmt::{Display, Formatter};
use std::path::PathBuf;
use std::str::FromStr;
use structopt::StructOpt;

/// Program to make sense of library of TV shows...
#[derive(StructOpt, Debug, PartialEq)]
#[structopt()]
pub enum Mode {
    Init{
    },
    RenameShow{
        #[structopt(short, long)]
        id: i32
    },
    RenameAllShows,
    SetLibrary{
        #[structopt(short, long)]
        id: i32,
        #[structopt(short, long)]
        scan: Option<bool>
    },
    AddScanDirectory{
        #[structopt(short, long)]
        path: PathBuf
    },
    AddShow{
        #[structopt(short, long)]
        name: String,
        #[structopt(short, long)]
        path: Option<PathBuf>,
        #[structopt(short, long,parse(try_from_str) ,default_value = "false")]
        risky: bool
    },
    RemoveShow{
        #[structopt(short, long)]
        id: i32
    },
    ListShows,
    ListScanDirectories,
}


#[derive(Debug)]
pub enum ParseError {
    InvalidMode,
}

impl Display for ParseError {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        match self {
            ParseError::InvalidMode => write!(f, "Invalid mode"),
        }
    }
}

