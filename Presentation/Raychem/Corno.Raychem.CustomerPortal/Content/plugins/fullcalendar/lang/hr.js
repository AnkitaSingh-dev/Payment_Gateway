﻿(function(e) { "function" == typeof define && define.amd ? define(["jquery", "moment"], e) : e(jQuery, moment) }
)(function(e, t) {
    function a(e, t, a) {
        var n = e + " ";
        switch (a) {
        case"m":
            return t ? "jedna minuta" : "jedne minute";
        case"mm":
            return n += 1 === e ? "minuta" : 2 === e || 3 === e || 4 === e ? "minute" : "minuta";
        case"h":
            return t ? "jedan sat" : "jednog sata";
        case"hh":
            return n += 1 === e ? "sat" : 2 === e || 3 === e || 4 === e ? "sata" : "sati";
        case"dd":
            return n += 1 === e ? "dan" : "dana";
        case"MM":
            return n += 1 === e ? "mjesec" : 2 === e || 3 === e || 4 === e ? "mjeseca" : "mjeseci";
        case"yy":
            return n += 1 === e ? "godina" : 2 === e || 3 === e || 4 === e ? "godine" : "godina"
        }
    }

    t.lang("hr",
    {
        months: "sječanj_veljača_ožujak_travanj_svibanj_lipanj_srpanj_kolovoz_rujan_listopad_studeni_prosinac"
            .split("_"),
        monthsShort: "sje._vel._ožu._tra._svi._lip._srp._kol._ruj._lis._stu._pro.".split("_"),
        weekdays: "nedjelja_ponedjeljak_utorak_srijeda_četvrtak_petak_subota".split("_"),
        weekdaysShort: "ned._pon._uto._sri._čet._pet._sub.".split("_"),
        weekdaysMin: "ne_po_ut_sr_če_pe_su".split("_"),
        longDateFormat: {
            LT: "H:mm",
            L: "DD. MM. YYYY",
            LL: "D. MMMM YYYY",
            LLL: "D. MMMM YYYY LT",
            LLLL: "dddd, D. MMMM YYYY LT"
        },
        calendar: {
            sameDay: "[danas u] LT",
            nextDay: "[sutra u] LT",
            nextWeek: function() {
                switch (this.day()) {
                case 0:
                    return"[u] [nedjelju] [u] LT";
                case 3:
                    return"[u] [srijedu] [u] LT";
                case 6:
                    return"[u] [subotu] [u] LT";
                case 1:
                case 2:
                case 4:
                case 5:
                    return"[u] dddd [u] LT"
                }
            },
            lastDay: "[jučer u] LT",
            lastWeek: function() {
                switch (this.day()) {
                case 0:
                case 3:
                    return"[prošlu] dddd [u] LT";
                case 6:
                    return"[prošle] [subote] [u] LT";
                case 1:
                case 2:
                case 4:
                case 5:
                    return"[prošli] dddd [u] LT"
                }
            },
            sameElse: "L"
        },
        relativeTime: {
            future: "za %s",
            past: "prije %s",
            s: "par sekundi",
            m: a,
            mm: a,
            h: a,
            hh: a,
            d: "dan",
            dd: a,
            M: "mjesec",
            MM: a,
            y: "godinu",
            yy: a
        },
        ordinal: "%d.",
        week: { dow: 1, doy: 7 }
    }), e.fullCalendar.datepickerLang("hr",
        "hr",
        {
            closeText: "Zatvori",
            prevText: "&#x3C;",
            nextText: "&#x3E;",
            currentText: "Danas",
            monthNames: ["Siječanj", "Veljača", "Ožujak", "Travanj", "Svibanj", "Lipanj", "Srpanj", "Kolovoz", "Rujan", "Listopad", "Studeni", "Prosinac"],
            monthNamesShort: ["Sij", "Velj", "Ožu", "Tra", "Svi", "Lip", "Srp", "Kol", "Ruj", "Lis", "Stu", "Pro"],
            dayNames: ["Nedjelja", "Ponedjeljak", "Utorak", "Srijeda", "Četvrtak", "Petak", "Subota"],
            dayNamesShort: ["Ned", "Pon", "Uto", "Sri", "Čet", "Pet", "Sub"],
            dayNamesMin: ["Ne", "Po", "Ut", "Sr", "Če", "Pe", "Su"],
            weekHeader: "Tje",
            dateFormat: "dd.mm.yy.",
            firstDay: 1,
            isRTL: !1,
            showMonthAfterYear: !1,
            yearSuffix: ""
        }), e.fullCalendar.lang("hr",
    { buttonText: { month: "Mjesec", week: "Tjedan", day: "Dan", list: "Raspored" }, allDayText: "Cijeli dan" })
});