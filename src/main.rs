use ansi_term::Colour::{Green, Red};
use ansi_term::Style;

use log::error;
use simplelog::ConfigBuilder;

use std::fs::OpenOptions;
use std::io;
use std::path::{Path, PathBuf};

use structopt::StructOpt;

use crate::api::show::ShowResult;
use crate::api::tv_maze;
use crate::args::args_parser::Mode;
use crate::database::database::Database;
use crate::database::models::{Episode, Show};
use crate::renamer::Renamer;
use crate::tv_maze::TVMaze;

mod api;
mod args;
mod database;
mod helper;
mod renamer;

fn main() {
    let (mode, mut db) = match init() {
        Some((mode, db)) => (mode, db),
        None => return,
    };
    match mode {
        Mode::AddShow { name, path, risky } => add_show(&mut db, &name, path, risky),
        Mode::ListShows => list_shows(&mut db),
        Mode::AddScanDirectory { path } => add_scan_directory(&mut db, path),
        Mode::ListScanDirectories => list_scan_directories(&mut db),
        Mode::RemoveShow { id } => remove_show(&mut db, id as i64),
        Mode::RenameShow { id } => rename_show(db, id as i64),
        Mode::RenameAllShows => rename_all_shows(db),
        Mode::Init {} => {
            println!(
                "{}",
                Red.paint("You just tried to initialize already initialized app...")
            );
        }
    }
}

fn rename_show(db: Database, id: i64) {
    let mut renamer = Renamer::new(&db);

    if let Err(e) = renamer.rename_show(id) {
        error!("{e}");
        eprintln!("{}", Red.paint(format!("{e}")));
    }
}

fn rename_all_shows(db: Database) {
    let mut renamer = Renamer::new(&db);

    if let Err(e) = renamer.rename_all_shows() {
        error!("{e}");
        eprintln!("{}", Red.paint(format!("{e}")));
    }
}

fn remove_show(db: &mut Database, id: i64) {
    let show = db.get_show(id);
    if let Some(show) = show {
        println!("{}", Green.paint(format!("Removed show: {}", show.name)));
        db.remove_show(id);
        db.save();
    } else {
        println!("{}", Red.paint(format!("Show with id {} not found", id)));
    }
}

fn list_scan_directories(db: &mut Database) {
    println!("{}", Green.paint("Scan directories:"));
    for dir in db.scan_dirs.iter() {
        println!("{}", dir.to_str().unwrap());
    }
}

fn add_scan_directory(db: &mut Database, path: PathBuf) {
    if db.scan_dirs.contains(&path) {
        println!("{}", Red.paint("Directory already added."));
        return;
    }
    if path.exists() {
        db.scan_dirs.push(path);
        db.save();
        println!("{}", Green.paint("Directory added to scan list."));
    } else {
        println!("{}", Red.paint("Directory does not exist."));
    }
}

fn list_shows(db: &mut Database) {
    println!("{}", Green.paint("All shows in the library:"));
    for show in &db.shows {
        println!(
            "{} (id: {}): \"{}\"",
            Style::new().bold().paint(&show.name),
            show.id,
            show.path.to_str().unwrap()
        );
    }
}

fn add_show(db: &mut Database, name: &String, path: Option<PathBuf>, risky: bool) {
    let show = match TVMaze::search(name, risky) {
        Ok(value) => value,
        Err(err) => {
            println!("{err}");
            return;
        }
    };
    if db.shows.iter().any(|x| x.id == show.id) {
        println!("{}", Red.paint("Show already exists in database."));
        return;
    }
    let episodes = match TVMaze::get_episodes(show.id) {
        Ok(value) => value,
        Err(err) => {
            println!("{err}");
            return;
        }
    };
    let mut show = Show::from(show);
    show.episodes = episodes.into_iter().map(Episode::from).collect();
    db.add_show(show, path);
    println!("\n{}", Green.paint("Show added to library."));
    db.save();
}

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
    init_log();

    let mode = Mode::from_args();
    let mut db = Database::new();
    let initialized = db.load();
    if initialized {
        return Some((mode, db));
    }
    if !matches!(mode, Mode::Init { .. }) {
        println!(
            "{}",
            Red.paint("You need to perform initialization before first use.")
        );
        println!("Use {} command.", Red.paint("init"));
        return None;
    }
    println!(
        "{}",
        Green.paint("Welcome to the revived version of TVS-Renamer.")
    );
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
    println!(
        "{}",
        Green.paint("You can now use the application. Have fun!")
    );
    println!(
        "{}",
        Red.italic()
            .bold()
            .paint("Also I'm not responsible for the app ruining your things.")
    );
    db.initialized = true;
    db.lib_dir = path;
    db.save();

    None
}

fn init_log() {
    let file = OpenOptions::new()
        .read(true)
        .append(true)
        .create(true)
        .open("log.txt")
        .unwrap();

    simplelog::WriteLogger::init(
        log::LevelFilter::Info,
        ConfigBuilder::new().add_filter_allow_str("tvsr").build(),
        file,
    )
    .unwrap()
}
