use crate::args::args_parser::Mode;
use crate::database::Database;
use structopt::StructOpt;

mod args;
mod database;


fn init() -> (Mode, Database) {
    let mode = Mode::from_args();
    let mut db = Database::new();
    db.load();
    return (mode, db);
}

fn main() {
    let (mode, db) = init();
    match mode {
        Mode::AddShow { name , path } => {

        }
        _ => todo!(),
    }
}
