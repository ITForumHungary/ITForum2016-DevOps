import { Component } from "@angular/core";
import { Http } from "@angular/http";

@Component({
    selector: "home",
    template: require("./home.component.html"),
    styles: [require("./home.component.css")]
})
export class HomeComponent {
    public result: Result;

    constructor(http: Http) {
        http.get("/api/vote/result").subscribe(result => {
            this.result = result.json();
        });
    }
}
interface Result {
    likes: number;
    disklikes: number;
    total: number;
    uniqueLikes: number;
    uniqueDislikes: number;
    uniqueTotal: number;
}
