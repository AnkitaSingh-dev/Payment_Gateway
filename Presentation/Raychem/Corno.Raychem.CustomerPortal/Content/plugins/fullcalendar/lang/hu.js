(function(e) { "function" == typeof define && define.amd ? define(["jquery", "moment"], e) : e(jQuery, moment) }
)(function(e, t) {
    function a(e, t, a, n) {
        var r = e;
        switch (a) {
        case"s":
            return n || t ? "néhány másodperc" : "néhány másodperce";
        case"m":
            return"egy" + (n || t ? " perc" : " perce");
        case"mm":
            return r + (n || t ? " perc" : " perce");
        case"h":
            return"egy" + (n || t ? " óra" : " órája");
        case"hh":
            return r + (n || t ? " óra" : " órája");
        case"d":
            return"egy" + (n || t ? " nap" : " napja");
        case"dd":
            return r + (n || t ? " nap" : " napja");
        case"M":
            return"egy" + (n || t ? " hónap" : " hónapja");
        case"MM":
            return r + (n || t ? " hónap" : " hónapja");
        case"y":
            return"egy" + (n || t ? " év" : " éve");
        case"yy":
            return r + (n || t ? " év" : " éve")
        }
        return""
    }

    function n(e) { return(e ? "" : "[múlt] ") + "[" + r[this.day()] + "] LT[-kor]" }

    var r = "vasárnap hétfőn kedden szerdán csütörtökön pénteken szombaton".split(" ");
    t.lang("hu",
    {
        months: "január_február_március_április_május_június_július_augusztus_szeptember_október_november_december"
            .split("_"),
        monthsShort: "jan_feb_márc_ápr_máj_jún_júl_aug_szept_okt_nov_dec".split("_"),
        weekdays: "vasárnap_hétfő_kedd_szerda_csütörtök_péntek_szombat".split("_"),
        weekdaysShort: "vas_hét_kedd_sze_csüt_pén_szo".split("_"),
        weekdaysMin: "v_h_k_sze_cs_p_szo".split("_"),
        longDateFormat: {
            LT: "H:mm",
            L: "YYYY.MM.DD.",
            LL: "YYYY. MMMM D.",
            LLL: "YYYY. MMMM D., LT",
            LLLL: "YYYY. MMMM D., dddd LT"
        },
        calendar: {
            sameDay: "[ma] LT[-kor]",
            nextDay: "[holnap] LT[-kor]",
            nextWeek: function() { return n.call(this, !0) },
            lastDay: "[tegnap] LT[-kor]",
            lastWeek: function() { return n.call(this, !1) },
            sameElse: "L"
        },
        relativeTime: {
            future: "%s múlva",
            past: "%s",
            s: a,
            m: a,
            mm: a,
            h: a,
            hh: a,
            d: a,
            dd: a,
            M: a,
            MM: a,
            y: a,
            yy: a
        },
        ordinal: "%d.",
        week: { dow: 1, doy: 7 }
    }), e.fullCalendar.datepickerLang("hu",
        "hu",
        {
            closeText: "bezár",
            prevText: "vissza",
            nextText: "előre",
            currentText: "ma",
            monthNames: ["Január", "Február", "Március", "Április", "Május", "Június", "Július", "Augusztus", "Szeptember", "Október", "November", "December"],
            monthNamesShort: ["Jan", "Feb", "Már", "Ápr", "Máj", "Jún", "Júl", "Aug", "Szep", "Okt", "Nov", "Dec"],
            dayNames: ["Vasárnap", "Hétfő", "Kedd", "Szerda", "Csütörtök", "Péntek", "Szombat"],
            dayNamesShort: ["Vas", "Hét", "Ked", "Sze", "Csü", "Pén", "Szo"],
            dayNamesMin: ["V", "H", "K", "Sze", "Cs", "P", "Szo"],
            weekHeader: "Hét",
            dateFormat: "yy.mm.dd.",
            firstDay: 1,
            isRTL: !1,
            showMonthAfterYear: !0,
            yearSuffix: ""
        }), e.fullCalendar.lang("hu",
    { buttonText: { month: "Hónap", week: "Hét", day: "Nap", list: "Napló" }, allDayText: "Egész nap" })
});