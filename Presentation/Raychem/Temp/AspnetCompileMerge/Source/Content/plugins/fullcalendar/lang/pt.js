﻿(function(e) { "function" == typeof define && define.amd ? define(["jquery", "moment"], e) : e(jQuery, moment) }
)(function(e, t) {
    t.lang("pt",
    {
        months: "Janeiro_Fevereiro_Março_Abril_Maio_Junho_Julho_Agosto_Setembro_Outubro_Novembro_Dezembro".split("_"),
        monthsShort: "Jan_Fev_Mar_Abr_Mai_Jun_Jul_Ago_Set_Out_Nov_Dez".split("_"),
        weekdays: "Domingo_Segunda-feira_Terça-feira_Quarta-feira_Quinta-feira_Sexta-feira_Sábado".split("_"),
        weekdaysShort: "Dom_Seg_Ter_Qua_Qui_Sex_Sáb".split("_"),
        weekdaysMin: "Dom_2ª_3ª_4ª_5ª_6ª_Sáb".split("_"),
        longDateFormat: {
            LT: "HH:mm",
            L: "DD/MM/YYYY",
            LL: "D [de] MMMM [de] YYYY",
            LLL: "D [de] MMMM [de] YYYY LT",
            LLLL: "dddd, D [de] MMMM [de] YYYY LT"
        },
        calendar: {
            sameDay: "[Hoje às] LT",
            nextDay: "[Amanhã às] LT",
            nextWeek: "dddd [às] LT",
            lastDay: "[Ontem às] LT",
            lastWeek: function() { return 0 === this.day() || 6 === this.day() ? "[Último] dddd [às] LT" : "[Última] dddd [às] LT" },
            sameElse: "L"
        },
        relativeTime: {
            future: "em %s",
            past: "%s atrás",
            s: "segundos",
            m: "um minuto",
            mm: "%d minutos",
            h: "uma hora",
            hh: "%d horas",
            d: "um dia",
            dd: "%d dias",
            M: "um mês",
            MM: "%d meses",
            y: "um ano",
            yy: "%d anos"
        },
        ordinal: "%dº",
        week: { dow: 1, doy: 4 }
    }), e.fullCalendar.datepickerLang("pt",
        "pt",
        {
            closeText: "Fechar",
            prevText: "&#x3C;Anterior",
            nextText: "Seguinte",
            currentText: "Hoje",
            monthNames: ["Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"],
            monthNamesShort: ["Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez"],
            dayNames: ["Domingo", "Segunda-feira", "Terça-feira", "Quarta-feira", "Quinta-feira", "Sexta-feira", "Sábado"],
            dayNamesShort: ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb"],
            dayNamesMin: ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb"],
            weekHeader: "Sem",
            dateFormat: "dd/mm/yy",
            firstDay: 0,
            isRTL: !1,
            showMonthAfterYear: !1,
            yearSuffix: ""
        }), e.fullCalendar.lang("pt",
    { buttonText: { month: "Mês", week: "Semana", day: "Dia", list: "Agenda" }, allDayText: "Todo o dia" })
});