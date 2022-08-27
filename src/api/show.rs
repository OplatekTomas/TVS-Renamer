use crate::helper::*;
use crate::TVMaze;
use ansi_term::Colour::{Green, Red};
use ansi_term::Style;
use ureq::Error;
use serde::Deserialize;
use serde::Serialize;

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct SearchResult {
    pub score: f64,
    pub show: ShowResult,
}

#[derive(Default, Debug, Clone, PartialEq, Eq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct ShowResult {
    pub id: i64,
    pub url: String,
    pub name: String,
    pub premiered: Option<String>,
}

impl TVMaze {
    pub fn search(name: &String, risky: bool) -> Result<ShowResult, Error> {
        if risky {
            //Returns the most probable result.
            let query = format!("https://api.tvmaze.com/singlesearch/shows?q={}", name);
            return ureq::get(&query).call()?.into_json().map_err(|e| e.into());
        }
        //Queries all the shows and lets user select the correct one.
        let query = format!("https://api.tvmaze.com/search/shows?q={}", name);
        let mut result: Vec<SearchResult> = ureq::get(&query).call()?.into_json()?;
        println!(
            "{}",
            Green.bold().paint("The API found the following shows:")
        );
        for (index, item) in result.iter().enumerate() {
            let bold = format!(
                "[{}]: {}",
                Style::new().bold().paint(index.to_string()),
                Style::new().bold().paint(&item.show.name)
            );
            match item.show.premiered {
                Some(ref premiered_date) => {
                    println!("{} ({}), {}", bold, premiered_date, item.show.url)
                }
                None => println!("{}, {}", bold, item.show.url),
            }
        }
        let text = format!("Please select a show (0-{}): ", result.len() - 1);
        let mut index: usize;
        loop {
            index = read_number(Some(format!("{}", Green.bold().paint(&text)))) as usize;
            if index < result.len() {
                break;
            }
            println!("{}", Red.paint("value not in range"));
        }

        Ok(result.remove(index).show)
    }
}
