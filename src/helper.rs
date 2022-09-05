use ansi_term::Colour::Red;
use std::io;
use std::io::Write;

fn print_before_read(text: &Option<String>) {
    match text {
        Some(text) => {
            print!("{}", text);
            io::stdout().flush().unwrap();
        }
        None => {}
    };
}

pub fn read_number(text: Option<String>) -> i32 {
    loop {
        print_before_read(&text);
        //What the fuck rust stdlib
        let mut input = String::new();
        io::stdin().read_line(&mut input).unwrap();
        let result = input.trim().parse();
        match result {
            Ok(value) => return value,
            Err(err) => {
                println!("{}", Red.paint(err.to_string()));
            }
        }
    }
}
