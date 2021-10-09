﻿(function(e) { "function" == typeof define && define.amd ? define(["jquery", "moment"], e) : e(jQuery, moment) }
)(function(e, t) {
    function a(e) { return e > 1 && 5 > e && 1 !== ~~(e / 10) }

    function n(e, t, n, r) {
        var i = e + " ";
        switch (n) {
        case"s":
            return t || r ? "pár vteřin" : "pár vteřinami";
        case"m":
            return t ? "minuta" : r ? "minutu" : "minutou";
        case"mm":
            return t || r ? i + (a(e) ? "minuty" : "minut") : i + "minutami";
        case"h":
            return t ? "hodina" : r ? "hodinu" : "hodinou";
        case"hh":
            return t || r ? i + (a(e) ? "hodiny" : "hodin") : i + "hodinami";
        case"d":
            return t || r ? "den" : "dnem";
        case"dd":
            return t || r ? i + (a(e) ? "dny" : "dní") : i + "dny";
        case"M":
            return t || r ? "měsíc" : "měsícem";
        case"MM":
            return t || r ? i + (a(e) ? "měsíce" : "měsíců") : i + "měsíci";
        case"y":
            return t || r ? "rok" : "rokem";
        case"yy":
            return t || r ? i + (a(e) ? "roky" : "let") : i + "lety"
        }
    }

    var r = "leden_únor_březen_duben_květen_červen_červenec_srpen_září_říjen_listopad_prosinec".split("_"),
        i = "led_úno_bře_dub_kvě_čvn_čvc_srp_zář_říj_lis_pro".split("_");
    t.lang("cs",
    {
        months: r,
        monthsShort: i,
        monthsParse: function(e, t) {
            var a, n = [];
            for (a = 0; 12 > a; a++) n[a] = RegExp("^" + e[a] + "$|^" + t[a] + "$", "i");
            return n
        }(r, i),
        weekdays: "neděle_pondělí_úterý_středa_čtvrtek_pátek_sobota".split("_"),
        weekdaysShort: "ne_po_út_st_čt_pá_so".split("_"),
        weekdaysMin: "ne_po_út_st_čt_pá_so".split("_"),
        longDateFormat: {
            LT: "H:mm",
            L: "DD.MM.YYYY",
            LL: "D. MMMM YYYY",
            LLL: "D. MMMM YYYY LT",
            LLLL: "dddd D. MMMM YYYY LT"
        },
        calendar: {
            sameDay: "[dnes v] LT",
            nextDay: "[zítra v] LT",
            nextWeek: function() {
                switch (this.day()) {
                case 0:
                    return"[v neděli v] LT";
                case 1:
                case 2:
                    return"[v] dddd [v] LT";
                case 3:
                    return"[ve středu v] LT";
                case 4:
                    return"[ve čtvrtek v] LT";
                case 5:
                    return"[v pátek v] LT";
                case 6:
                    return"[v sobotu v] LT"
                }
            },
            lastDay: "[včera v] LT",
            lastWeek: function() {
                switch (this.day()) {
                case 0:
                    return"[minulou neděli v] LT";
                case 1:
                case 2:
                    return"[minulé] dddd [v] LT";
                case 3:
                    return"[minulou středu v] LT";
                case 4:
                case 5:
                    return"[minulý] dddd [v] LT";
                case 6:
                    return"[minulou sobotu v] LT"
                }
            },
            sameElse: "L"
        },
        relativeTime: {
            future: "za %s",
            past: "před %s",
            s: n,
            m: n,
            mm: n,
            h: n,
            hh: n,
            d: n,
            dd: n,
            M: n,
            MM: n,
            y: n,
            yy: n
        },
        ordinal: "%d.",
        week: { dow: 1, doy: 4 }
    }), e.fullCalendar.datepickerLang("cs",
        "cs",
        {
            closeText: "Zavřít",
            prevText: "&#x3C;Dříve",
            nextText: "Později&#x3E;",
            currentText: "Nyní",
            monthNames: ["leden", "únor", "březen", "duben", "květen", "červen", "červenec", "srpen", "září", "říjen", "listopad", "prosinec"],
            monthNamesShort: ["led", "úno", "bře", "dub", "kvě", "čer", "čvc", "srp", "zář", "říj", "lis", "pro"],
            dayNames: ["neděle", "pondělí", "úterý", "středa", "čtvrtek", "pátek", "sobota"],
            dayNamesShort: ["ne", "po", "út", "st", "čt", "pá", "so"],
            dayNamesMin: ["ne", "po", "út", "st", "čt", "pá", "so"],
            weekHeader: "Týd",
            dateFormat: "dd.mm.yy",
            firstDay: 1,
            isRTL: !1,
            showMonthAfterYear: !1,
            yearSuffix: ""
        }), e.fullCalendar.lang("cs",
    { buttonText: { month: "Měsíc", week: "Týden", day: "Den", list: "Agenda" }, allDayText: "Celý den" })
});