use std::path::PathBuf;
use structopt::StructOpt;

/// Program to make sense of library of TV shows...
#[derive(StructOpt, Debug, PartialEq, Eq)]
#[structopt()]
pub enum Mode {
    Init {},
    RenameShow {
        #[structopt(short, long)]
        id: i32,
    },
    RenameAllShows,
    AddScanDirectory {
        #[structopt(short, long)]
        path: PathBuf,
    },
    AddShow {
        #[structopt(short, long)]
        name: String,
        #[structopt(short, long)]
        path: Option<PathBuf>,
        #[structopt(short, long, parse(try_from_str), default_value = "false")]
        risky: bool,
    },
    RemoveShow {
        #[structopt(short, long)]
        id: i32,
    },
    ListShows,
    ListScanDirectories,
}
