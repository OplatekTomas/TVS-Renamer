use std::io;
use std::io::Read;
use ansi_term::Colour::{Green, Red};
use reqwest::Error;
use crate::args::args_parser::Mode;
use crate::database::Database;
use structopt::StructOpt;
use crate::api::search::ShowResult;
use crate::api::tv_maze;
use crate::tv_maze::TVMaze;
use std::path::{Path, PathBuf};

mod args;
mod database;
mod api;
mod helper;


fn read_path() -> PathBuf {
    let mut path = String::new();
    loop {
        path.clear();
        io::stdin().read_line(&mut path).unwrap();
        //let x = path.trim();
        if Path::new(path.trim()).exists() {
            break;
        }
        println!("Yeah so that was invalid af. Try again: ");
    }
    return path.trim().into();
}

fn init() -> Option<(Mode, Database)> {
    let mode = Mode::from_args();
    let mut db = Database::new();
    let initialized = db.load();
    if initialized {
        return Some((mode, db));
    }
    if !matches!(mode, Mode::Init{..}) {
        println!("{}", Red.paint("You need to perform initialization before first use."));
        println!("Use {} command.", Red.paint("init"));
        return None;
    }
    println!("{}", Green.paint("Welcome to the revived version of TVS-Renamer."));
    println!("The original version was a way to learn C#, well now I'm learning Rust...");
    println!("Compared to the original there is no UI, but there is a ton of extra functionality");
    println!();
    println!("{}", Green.paint("Lets begin by selecting a directory where your TV show library either will be, or is located at:"));
    let mut path: PathBuf;
    loop {
        path = read_path();
        println!("Selected path: {}. Are you sure? y/n", path.to_str()?);
        let mut answer = String::new();
        io::stdin().read_line(&mut answer).unwrap();
        if answer.to_lowercase().trim() == "y" {
            break;
        }
        println!("Select library path: ");
    }
    println!("{}", Green.paint("You can now use the application. Have fun!"));
    println!("{}", Red.italic().bold().paint("Also I'm not responsible for the app ruining your things."));
    db.initialized = true;
    db.lib_dir = path;
    db.save();
    return None;
}


fn main() {
    let (mode, db) = match init() {
        Some((mode, db)) => (mode, db),
        None => return
    };
    match mode {
        Mode::AddShow { name, path } => {
            let show = match TVMaze::search(&name) {
                Ok(value) => value,
                Err(err) => {
                    println!("{}", err.to_string());
                    return;
                }
            };
        }
        Mode::Init {} => {
            println!("{}", Red.paint("You just tried to initialize already initialized app..."));
        }
        _ => todo!(),
    }
}
