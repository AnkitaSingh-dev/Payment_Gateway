(function(c) {
    function p(a) {
        var b, h;
        b = Number.POSITIVE_INFINITY;
        h = Number.NEGATIVE_INFINITY;
        for (var f = 0; f < a.length; f++) b = Math.min(b, a[f][1]), h = Math.max(h, a[f][1]);
        return{ min: b, max: h, diff: h - b }
    }

    function m(a, b, h) {
        c("#FLOTtooltip").remove();
        c('<div id="FLOTtooltip">' + h + "</div>")
            .css({
                position: "absolute",
                display: "none",
                top: b + 5,
                left: a + 5,
                border: "1px solid #fdd",
                padding: "2px",
                "background-color": "#fee",
                opacity: 0.8
            })
            .appendTo("body")
            .fadeIn(200)
    }

    function x(a, b, h) {
        function f(a, d) { return a[1] - d[1] }

        function d(a,
            d) {
            return d[1] - a[1]
        }

        for (var e = [], k = 0; k < a.length; k++) e.push(a[k]);
        h ? e.sort(h) : "a" === b ? e.sort(f) : e.sort(d);
        return e
    }

    function t(a) {
        for (var b = [], h = 0; h < a.length; h++) a[h][2] ? b.push([a[h][0], a[h][2]]) : b.push(a[h][0], a[h][0]);
        return b
    }

    function q(a) {
        var b, h, f = [];
        b = p(a);
        for (var d = 0; d < a.length; d++) h = 100 * ((a[d][1] - b.min) / b.diff), f.push([a[d][0], h]);
        return f
    }

    function n(a, b) { for (var h in b) a[h] ? "object" === typeof b[h] && n(a[h], b[h]) : a[h] = b[h] }

    c.plot.JUMlib = {};
    c.plot.JUMlib.library = {};
    c.plot.JUMlib.library.between =
        function(a, b, h) { return h > b ? a >= b && a <= h : a >= h && a <= b };
    c.plot.JUMlib.library.getMinMax = p;
    c.plot.JUMlib.library.showHover = function(a, b, h, f, d) {
        if (h) {
            a = h.series.data[h.dataIndex];
            if (d) d = d(a);
            else if (d = "X:" + a[0] + "<br>Y:" + a[1], 2 < a.length) for (h = 2; h < a.length; h++) d += "<br>" + a[h];
            m(b.pageX, b.pageY, d)
        } else !0 === f ? (d = b.x1 + " / " + b.y1, m(b.pageX, b.pageY, d)) : c("#FLOTtooltip").remove()
    };
    c.plot.JUMlib.library.showTooltip = m;
    c.plot.JUMlib.prepareData = {};
    c.plot.JUMlib.prepareData.createQuartile = function(a, b, h) {
        var f = [],
            d = [],
            e = [],
            k = [],
            g = [],
            j = [],
            l,
            w,
            u,
            r,
            v;
        l = (0.25 * a.length).toFixed(0);
        w = (0.5 * a.length).toFixed(0);
        u = (0.75 * a.length).toFixed(0);
        r = a.length - 1;
        for (var c = 0; c < a[0].length; c++) {
            v = [];
            for (var A = 0; A < a.length; A++) v.push(a[A][c]);
            v.sort(function(a, g) { return a - g });
            d.push([c, v[l]]);
            e.push([c, v[w]]);
            k.push([c, v[u]]);
            g.push([c, v[r]]);
            f.push([c, v[0]]);
            j.push([c, a[b][c]])
        }
        return[
            { data: g }, { data: k }, { data: e }, { data: d }, { data: f, color: "#ffffff" },
            { label: h, points: { show: !0 }, lines: { fill: null, steps: !1 }, data: j }
        ]
    };
    c.plot.JUMlib.prepareData.createPercentile =
        function(a, b, h, f) {
            var d = [], e = [], k = [], g, j;
            if (f.length) {
                k.push([0]);
                d.push([]);
                for (g = 0; g < f.length; g++) k.push(parseInt(a.length * f[g], 0)), d.push([])
            } else for (g = 0; g < f; g++) k.push(parseInt(a.length / f * g, 0)), d.push([]);
            k.push(a.length - 1);
            d.push([]);
            for (g = 0; g < a[0].length; g++) {
                f = [];
                for (j = 0; j < a.length; j++) f.push(a[j][g]);
                f.sort(function(a, g) { return a - g });
                for (j = 0; j < d.length; j++) d[j].push([g, f[k[j]]]);
                e.push([g, a[b][g]])
            }
            a = [];
            for (g = d.length - 1; 0 < g; g--) a.push({ data: d[g] });
            a.push({ data: d[0], color: "#ffffff" });
            a.push({
                label: h,
                points: { show: !0 },
                lines: { fill: null, steps: !1 },
                data: e
            });
            return a
        };
    c.plot.JUMlib.prepareData.createSimiliarity = function(a, b, h) {
        var f = [];
        a = q(a);
        b = q(b);
        for (var d, e = 0; e < a.length; e++) {
            switch (h) {
            case "diff":
                d = a[e][1] - b[e][1];
                break;
            case "abs":
                d = Math.abs(a[e][1] - b[e][1]);
                break;
            default:
                d = 0
            }
            f.push([a[e][0], d])
        }
        return f
    };
    c.plot.JUMlib.prepareData.createWaterfall = function(a, b) {
        var h = [], f = [], d = [], e = [], k = 0, g = Number.POSITIVE_INFINITY, j;
        for (j = 0; j < a.length; j++)
            a[j][2]
                ? "undefined" === typeof a[j][1]
                ? h.push([j, k])
                : (h.push([
                    j,
                    a[j][1]
                ]), k = a[j][1])
                : 0 < a[j][1]
                ? (e.push([j, -a[j][1]]), k += a[j][1], f.push([j, k]), d.push([j, 0]))
                : (d.push([j, -a[j][1]]), k += a[j][1], f.push([j, k])), g = Math.min(g, k);
        k = [];
        for (j = 0; j < a.length; j++) k.push([j, a[j][0]]);
        return{
            data: [
                { data: h, color: b.fixed }, { data: f, bars: { show: !1 }, lines: { show: !1 } },
                { data: d, color: b.negative }, { data: e, color: b.positive }
            ],
            ticks: k,
            yaxismin: g
        }
    };
    c.plot.JUMlib.prepareData.avg = function(a, b) {
        for (var h = [], f = [], d, e, k = 0; k < a.length; k++) {
            d = k < b ? 0 : k - b + 1;
            f = [];
            f.push(a[k][0]);
            for (var g = 1; g < a[k].length; g++) {
                e =
                    0;
                for (var j = d; j <= k; j++) e += a[j][g];
                f.push(e / (k - d + 1))
            }
            h.push(f)
        }
        return h
    };
    c.plot.JUMlib.prepareData.max = function(a, b) {
        for (var h = [], f = [], d, e, k = 0; k < a.length; k++) {
            d = k < b ? 0 : k - b + 1;
            f = [];
            f.push(a[k][0]);
            for (var g = 1; g < a[k].length; g++) {
                e = -Number.MAX_VALUE;
                for (var j = d; j <= k; j++) a[j][g] > e && (e = a[j][g]);
                f.push(e)
            }
            h.push(f)
        }
        return h
    };
    c.plot.JUMlib.prepareData.min = function(a, b) {
        for (var h = [], f = [], d, e, k = 0; k < a.length; k++) {
            d = k < b ? 0 : k - b + 1;
            f = [];
            f.push(a[k][0]);
            for (var g = 1; g < a[k].length; g++) {
                e = Number.MAX_VALUE;
                for (var j = d;
                    j <=
                        k;
                    j++) a[j][g] < e && (e = a[j][g]);
                f.push(e)
            }
            h.push(f)
        }
        return h
    };
    c.plot.JUMlib.prepareData.sort = x;
    c.plot.JUMlib.prepareData.sortTicks = function(a, b, h) {
        a = x(a, b, h);
        for (b = 0; b < a.length; b++) a[b][0] = b;
        return{ data: a, ticks: t(a) }
    };
    c.plot.JUMlib.prepareData.pareto = function(a, b, h, f, d) {
        var e = [], k = "Others", g = !0, j = [], e = x(a, "d");
        0 < b.length && (k = b);
        g = h;
        if (f) {
            for (a = 0; a < f; a++) j.push(e[a]);
            d = 0;
            for (a = f; a < e.length; a++) d += e[a][1];
            g && j.push([f, d, k]);
            e = j
        } else if (d) {
            h = b = f = 0;
            var l;
            for (a = 0; a < e.length; a++) f += e[a][1];
            f = f * d / 100;
            for (a =
                    0;
                a < e.length;
                a++) b < f ? (j.push(e[a]), b += e[a][1], l = a) : h += e[a][1];
            l++;
            g && j.push([l, h, k]);
            e = j
        }
        for (a = 0; a < e.length; a++) e[a][0] = a;
        return{ data: e, ticks: t(e) }
    };
    c.plot.JUMlib.prepareData.normalize = q;
    c.plot.JUMlib.prepareData.combineData = function(a, b) {
        for (var h = [], f = 0; f < a.length; f++) {
            for (var d = [], e = 0; e < a[f].length; e++) d.push([b[e], a[f][e]]);
            h.push(d)
        }
        return h
    };
    c.plot.JUMlib.data = {};
    c.plot.JUMlib.data.createFont = function(a) {
        return{
            style: a.css("font-style"),
            size: Math.round(0.8 *
            (+a.css("font-size")
                .replace("px",
                    "") ||
                13)),
            variant: a.css("font-variant"),
            weight: a.css("font-weight"),
            family: a.css("font-family")
        }
    };
    c.plot.JUMlib.data.createColors = function(a, b) {
        for (var h, f = [], d = a.colors, e = d.length, k = 0, g = 0; g < e; g++) f[g] = d[g];
        if (e < b)
            for (g = e; g < b; g++)
                h = c.color
                    .parse(d[g % e] || "#666"), 0 === g % e && g && (k = 0 <= k ? 0.5 > k ? -k - 0.2 : 0 : -k), f[g] =
                    h.scale("rgb", 1 + k).toString();
        return f
    };
    c.plot.JUMlib.data.getColor = function(a) {
        function b(b) {
            var f;
            if ("object" === typeof b)
                if ("undefined" !== typeof b.image) f = a.ctx.createPattern(b.image, b.repeat);
                else {
                    f = a.radius
                        ? a.ctx.createRadialGradient(a.left, a.top, 0, a.left, a.top, a.radius)
                        : a.ctx.createLinearGradient(0, 0, a.width, a.height);
                    for (var d = 0; d < b.colors.length; d++) {
                        var e = b.colors[d];
                        "string" !== typeof e &&
                        (e = c.color.parse(a
                                .defaultColor), null !== b.brightness && e.scale("rgb", b.brightness),
                            null !== b.opacity && (e *= b.opacity), e = e.toString());
                        f.addColorStop(d / (b.colors.length - 1), e)
                    }
                }
            else f = "string" === typeof b ? b : a.colors[b];
            return f
        }

        return"object" === typeof a
            ? "undefined" !== typeof a.dataIndex
            ? "undefined" !== typeof a.serie.data[a.dataIndex].color
            ? b(a.serie.data[a.dataIndex].color)
            : a.colors[a.dataIndex]
            : "undefined" !== typeof a.serieIndex
            ? "undefined" !== typeof a.serie.color ? b(a.serie.color) : a.colors[a.serieIndex]
            : "undefined" !== typeof a.color ? b(a.color) : "darkgreen"
            : b(a)
    };
    c.plot.JUMlib.data.loadImages = function(a, b, h) {
        function f(a) {
            var e = c.Deferred(), l, w;
            w = a.path + a.name + "." + a.type;
            l = setInterval(function() {
                    clearInterval(l);
                    e.reject()
                },
                b);
            c("<img />")
                .attr("src", w)
                .load(function() {
                    d[a.name] = this;
                    e.resolve()
                })
                .error(function(l) {
                    console.log(w, l);
                    d[a.name] =
                        null;
                    e.reject()
                });
            return e.promise()
        }

        for (var d = {}, e = [], k = 0; k < a.length; k++) e.push(f(a[k]));
        c.when.apply(null, e).then(function() { h(d) })
    };
    c.plot.JUMlib.data.getCanvases = function(a) {
        return{
            background: c(a).children(".flot-background"),
            base: c(a).children(".flot-base"),
            overlay: c(a).children(".flot-overlay")
        }
    };
    c.plot.JUMlib.data.extendEmpty = n;
    c.plot.JUMlib.drawing = {};
    c.plot.JUMlib.drawing.drawLines = function(a, b) {
        var c, f, d;
        c = a.getPlotOffset();
        f = a.getData();
        d = a.getCanvas().getContext("2d");
        d.translate(c.left,
            c.top);
        for (c = 0; c < b.length; c++) {
            var e = f[b[c].from.seriesIndex], k = f[b[c].to.seriesIndex], g = b[c].from, j = b[c].to;
            g.dataFieldX || (g.dataFieldX = 0);
            g.dataFieldY || (g.dataFieldY = 1);
            j.dataFieldX || (j.dataFieldX = 0);
            j.dataFieldY || (j.dataFieldY = 1);
            var l, w;
            l = e.xaxis.p2c(e.data[g.dataIndex][g.dataFieldX]);
            g = e.yaxis.p2c(e.data[g.dataIndex][g.dataFieldY]);
            w = e.xaxis.p2c(k.data[j.dataIndex][j.dataFieldX]);
            e = e.yaxis.p2c(k.data[j.dataIndex][j.dataFieldY]);
            d.beginPath();
            d.strokeStyle = "red";
            d.lineWidth = 5;
            d.moveTo(l, g);
            d.lineTo(w,
                e);
            d.stroke()
        }
    }
})(jQuery);
(function(c) {
    c.plot.plugins.push({
        init: function(c) {
            function p(e) {
                e = a(e);
                if (e[1]) {
                    var b = e[1].series;
                    !1 === b.points.show &&
                        ((!b.lines.show || !1 === b.lines.show) && !1 === b.bars.show) &&
                        d.trigger("plotclick", e)
                }
            }

            function t(e) {
                if (b.grid.editable) {
                    e = a(e);
                    if (e[1]) {
                        var c = null;
                        if (e[1].series.editable) {
                            c = e[1].dataIndex.length
                                ? e[1].series.data[e[1].dataIndex[0]]
                                : e[1].series.data[e[1].dataIndex];
                            switch (e[1].series.editMode) {
                            case "x":
                                e[0].y = c[1];
                                e[0].y1 = c[1];
                                break;
                            case "y":
                                e[0].x = c[0], e[0].x1 = c[0]
                            }
                            b.series.justEditing =
                                e
                        }
                        d.trigger("plotdown", e)
                    }
                    d.css("cursor", "auto")
                }
            }

            function q() {
                if (b.series.justEditing) {
                    d.trigger("plotup", b.series.justEditing);
                    d.trigger("datadrop", b.series.justEditing);
                    var a = b.series.justEditing[1].series;
                    if (!0 === a.autoEdit) {
                        var f = b.series.justEditing[0];
                        a.data[a.dataIndex] = [f.x1, f.y1]
                    }
                }
                b.series.justEditing = null;
                c.triggerRedrawOverlay()
            }

            function n(e) {
                var f;
                e = a(e);
                if (b.series.justEditing)
                    switch (f = b.series.justEditing[1].seriesIndex, c.getData()[f].editMode) {
                    case "none":
                        break;
                    case "x":
                        b.series.justEditing[0].x =
                            e[0].x;
                        b.series.justEditing[0].x1 = e[0].x1;
                        b.series.justEditing[0].pageX = e[0].pageX;
                        break;
                    case "y":
                        b.series.justEditing[0].y = e[0].y;
                        b.series.justEditing[0].y1 = e[0].y1;
                        b.series.justEditing[0].pageY = e[0].pageY;
                        break;
                    case "v":
                        e[1] && (b.series.justEditing[0] = e[0], b.series.justEditing[0].value = e[1].value);
                        break;
                    case "xy":
                        b.series.justEditing[0] = e[0];
                        break;
                    default:
                        b.series.justEditing[0] = e[0]
                    }
                else if (e[1]) {
                    b.series.justMoving = e;
                    switch (e[1].series.editMode) {
                    case "x":
                        d.css("cursor", "col-resize");
                        break;
                    case "y":
                        d.css("cursor",
                            "row-resize");
                        break;
                    default:
                        d.css("cursor", "crosshair")
                    }
                    f = e[1].series;
                    !1 === f.points.show &&
                        ((!f.lines.show || !1 === f.lines.show) && !1 === f.bars.show) &&
                        d.trigger("plothover", e)
                } else d.css("cursor", "auto"), b.series.justMoving = null;
                c.triggerRedrawOverlay()
            }

            function a(a) {
                var d, g = null, j = h.offset(), l = a.pageX - j.left - f.left, w = a.pageY - j.top - f.top;
                d = c.c2p({ left: l, top: w });
                d.pageX = a.pageX;
                d.pageY = a.pageY;
                a = null;
                for (var b, g = 0; g < c.getData().length; g++) {
                    var r = c.getData()[g];
                    null !== r.nearBy.findItem &&
                    (a = r.nearBy.findItem(l,
                        w,
                        g,
                        r));
                    if (a) break
                }
                a
                    ? (g = a[0], l = a[1], w = c.getData()[g].datapoints
                        .pointsize, 2 < a.length && (b = a[2]), g =
                    {
                        datapoint: c.getData()[g].datapoints.points.slice(l * w, (l + 1) * w),
                        dataIndex: l,
                        series: c.getData()[g],
                        seriesIndex: g,
                        value: b
                    })
                    : g = null;
                g &&
                (g.pageX = parseInt(g.series.xaxis.p2c(g
                        .datapoint[0]) +
                    j.left +
                    c.getPlotOffset().left,
                    0), g.pageY = parseInt(g.series.yaxis.p2c(g.datapoint[1]) + j.top + c.getPlotOffset().top, 0));
                return[d, g]
            }

            var b = null, h = null, f = null, d = null;
            c.hooks.bindEvents.push(function(a, c) {
                b = a.getOptions();
                h = c;
                f = a.getPlotOffset();
                d = a.getPlaceholder();
                if (b.grid
                    .editable ||
                    b.grid.clickable ||
                    b.grid.hoverable) h.mousedown(t), h.mouseup(q), h.mousemove(n), h.click(p)
            });
            c.hooks.drawOverlay.push(function(a, d) {
                var g, j, l;
                d.save();
                d.clearRect(0, 0, a.getPlaceholder().width, a.getPlaceholder().height);
                d.translate(a.getPlotOffset().left, a.getPlotOffset().top);
                b.series.justEditing
                    ? (g = a.getData()[b.series.justEditing[1]
                        .seriesIndex], j = b.series.justEditing[0]
                        .x1, l = b.series.justEditing[0].y1, g.nearBy.drawEdit &&
                        g.nearBy.drawEdit(d,
                            j,
                            l,
                            g))
                    : b.series.justMoving &&
                    (g = a.getData()[b.series.justMoving[1]
                        .seriesIndex], g.nearBy
                        .drawHover &&
                        g.nearBy.drawHover(d, g, b.series.justMoving[1].dataIndex));
                d.restore()
            })
        },
        options: {
            series: {
                editable: null,
                editMode: "xy",
                justEditing: null,
                justMoving: null,
                autoSet: !1,
                nearBy: {
                    distance: 6,
                    findItem: function(c, x, t, q) {
                        var n = q.nearBy.distance,
                            a = n * n + 1,
                            b = null,
                            h,
                            f,
                            d = q.xaxis,
                            e = q.yaxis,
                            k = q.datapoints.points,
                            g = q.datapoints.pointsize,
                            j = d.c2p(c),
                            l = e.c2p(x),
                            w = n / d.scale,
                            u = n / e.scale;
                        d.options.inverseTransform && (w = Number.MAX_VALUE);
                        e.options.inverseTransform && (u = Number.MAX_VALUE);
                        for (n = 0; n < k.length; n += g)
                            if (h = k[n], f = k[n + 1], null !== h)
                                switch (q.nearBy.findMode) {
                                case "circle":
                                    if (h - j > w || h - j < -w) continue;
                                    if (f - l > u || f - l < -u) continue;
                                    h = Math.abs(d.p2c(h) - c);
                                    f = Math.abs(e.p2c(f) - x);
                                    f = h * h + f * f;
                                    f < a && (a = f, b = [t, n / g]);
                                    break;
                                case "vertical":
                                    p(c, d.p2c(h), d.p2c(h + q.nearBy.width)) &&
                                    (f = Math.abs(e.p2c(f) - x), f < q.nearBy.distance && (b = [t, n / g]));
                                    break;
                                case "horizontal":
                                    var r = 0 > e.datamin ? Math.max(0, e.datamin) : Math.min(0, e.datamin);
                                    p(x, e.p2c(f), e.p2c(r)) &&
                                    (f =
                                        Math.abs(d.p2c(h) - c), f <= q.nearBy.distance && (b = [t, n / g]))
                                }
                        return b
                    },
                    findMode: "circle",
                    drawEdit: function(m, p, t, q) {
                        var n = q.xaxis, a = q.yaxis;
                        if (!(p < n.min || p > n.max || t < a.min || t > a.max))
                            switch (q.nearBy.findMode) {
                            case "circle":
                                var b = q.points.radius + q.points.lineWidth / 2;
                                m.lineWidth = b;
                                m.strokeStyle = c.color.parse(q.color).scale("a", 0.5).toString();
                                q = 1.5 * b;
                                p = n.p2c(p);
                                t = a.p2c(t);
                                m.beginPath();
                                m.arc(p, t, q, 0, 2 * Math.PI, !1);
                                m.fillStyle = "#ff8080";
                                m.fill();
                                m.lineWidth = 2;
                                m.moveTo(p, t - q);
                                m.lineTo(p, t + q);
                                m.moveTo(p - q, t);
                                m.lineTo(p + q, t);
                                m.closePath();
                                m.stroke();
                                break;
                            case "vertical":
                                m.lineWidth = 2;
                                m.strokeStyle = c.color.parse(q.color).scale("a", 0.5).toString();
                                m.beginPath();
                                m.moveTo(n.p2c(p), a.p2c(t));
                                m.lineTo(n.p2c(p + q.nearBy.width), a.p2c(t));
                                m.closePath();
                                m.stroke();
                                break;
                            case "horizontal":
                                m.lineWidth = 4, m.strokeStyle = c.color.parse(q.color).scale("a", 0.5).toString(), m
                                    .beginPath(), m.moveTo(n
                                    .p2c(p),
                                    a.p2c(t)), t = 0 > a.datamin ? Math.max(0, a.datamin) : Math.min(0, a.datamin), m
                                    .lineTo(n.p2c(p), a.p2c(t)), m.closePath(), m.stroke()
                            }
                    },
                    drawHover: function() {}
                }
            },
            grid: { editable: !1 }
        },
        name: "mouse",
        version: "0.2"
    });
    var p = c.plot.JUMlib.library.between
})(jQuery);
(function(c) {
    var p = "background",
        m = {
            grid: {
                background: {
                    active: !1,
                    mode: "color",
                    color: { colors: ["white", "yellow", "orange", "blue"] },
                    image: null,
                    fncDraw: null,
                    setZIndex: !1,
                    debug: { active: !1, createDocuTemplate: null }
                },
                overlay: { active: !1, image: null, opacity: 0.2 }
            }
        };
    c.plot.plugins.push({
        init: function(x, t) {
            function q() {
                var a, d;
                a = c.plot.JUMExample.docuObjectToTemplate([
                        { name: "options.grid.background", tree: e.grid.background, takeDefault: !0 },
                        { name: "options.grid.background", tree: m.grid.background, takeDefault: !0 }, {
                            name: "options.grid.overlay",
                            tree: e.grid.overlay,
                            takeDefault: !0
                        }, { name: "options.grid.overlay", tree: m.grid.overlay, takeDefault: !0 }
                    ],
                    p);
                c.plot.JUMExample.extendDocuObject(a, p);
                d = c.plot.JUMExample.docuObjectToEdit(a, "");
                return{ data: a, form: d }
            }

            function n(g) {
                e = g.getOptions();
                var j = e.grid.background.setZIndex;
                f = new h("flot-background", g.getPlaceholder());
                !0 === c.isNumeric(j)
                    ? (c(g
                            .getPlaceholder()
                            .children(".flot-overlay"))
                        .css("z-index", j + 1), c(g.getCanvas()).css("z-index", j), c(f.element).css("z-index", j - 1))
                    : !0 === e.grid.background.setZIndex
                    ? (c(g
                            .getPlaceholder()
                            .children(".flot-overlay"))
                        .css("z-index", 2), c(g.getCanvas()).css("z-index", 1), c(f.element).css("z-index", 0))
                    : c(f.element).css("z-index", -1);
                d = f.context;
                k = g.getPlotOffset();
                d.save();
                d.translate(k.left, k.top);
                switch (e.grid.background.mode) {
                case "image":
                    j = e.grid.background.image;
                    "undefined" !== typeof j && d.drawImage(j, 0, 0, g.width(), g.height());
                    break;
                case "color":
                    a(g, d);
                    break;
                case "userdefined":
                    e.grid.background.fncDraw(g, d, g.width(), g.height());
                    break;
                default:
                    a(g, d)
                }
                d.restore()
            }

            function a(a,
                d) {
                var l = c.plot.JUMlib.data.getColor({
                    ctx: d,
                    color: e.grid.background.color,
                    left: 0,
                    top: 0,
                    height: a.height(),
                    width: a.width()
                });
                d.fillStyle = l;
                d.fillRect(0, 0, a.width(), a.height())
            }

            function b(a) {
                var d = e.grid.overlay.image,
                    l = '<div style="position:absolute;width:' +
                        a.width() +
                        ";height:" +
                        a.height() +
                        ";top:" +
                        k.top +
                        ";left:" +
                        k.left +
                        ';">',
                    l = c(l);
                c(d).css("opacity", e.grid.overlay.opacity).width(a.width()).height(a.height());
                c(d).css("top", k.top).css("position", "absolute").css("left", k.left);
                c(d).appendTo(l);
                l.appendTo(a.getPlaceholder())
            }

            var h, f, d, e, k;
            h = t.Canvas;
            x.hooks.processOptions.push(function(a, d) {
                !0 === d.grid.background.active &&
                (e = d, a.hooks.drawBackground
                        .push(n), !0 === d.grid.overlay.active && a.hooks.draw.push(b),
                    !0 === d.grid.background.debug.active && (e.grid.background.debug.createDocuTemplate = q))
            })
        },
        options: m,
        name: p,
        version: "0.4"
    })
})(jQuery);
(function(c) {
    var p = "bandwidth",
        m = {
            series: {
                bandwidth: {
                    active: !1,
                    show: !1,
                    fill: !0,
                    lineWidth: "4px",
                    highlight: { opacity: 0.5 },
                    drawBandwidth: function(c, a, b, h, f, d) {
                        c.beginPath();
                        c.strokeStyle = d;
                        c.lineWidth = a.barWidth;
                        c.lineCap = "round";
                        c.moveTo(b, h);
                        c.lineTo(b, f);
                        c.stroke()
                    },
                    debug: { active: !1, createDocuTemplate: null }
                }
            }
        },
        x = { series: { lines: { show: !1 } } },
        t = { series: { editMode: "y", nearBy: { distance: 7, findItem: null, findMode: "circle", drawHover: null } } },
        q = c.plot.JUMlib.library.between;
    c.plot.plugins.push({
        init: function(n) {
            function a(a,
                g) {
                !0 === g.bandwidth.show && (g.nearBy.findItem = d, g.nearBy.drawHover = e)
            }

            function b() {
                var a, d;
                a = c.plot.JUMExample.docuObjectToTemplate([
                        { name: "data", tree: j.data }, {
                            name: "options.series.bandwidth",
                            tree: m.series.bandwidth,
                            takeDefault: !0
                        },
                        { name: "options.series.bandwidth", tree: g.series.bandwidth },
                        { name: "options.series.editMode", tree: m.series.editMode, takeDefault: !0 },
                        { name: "options.series.editMode", tree: g.series.editMode },
                        { name: "options.series.nearBy", tree: m.series.nearBy, takeDefault: !0 }, {
                            name: "options.series.nearBy",
                            tree: g.series.nearBy
                        }
                    ],
                    p);
                c.plot.JUMExample.extendDocuObject(a, p);
                d = c.plot.JUMExample.docuObjectToEdit(a, "");
                return{ data: a, form: d }
            }

            function h(a, d, e) {
                if (e.bandwidth.show) {
                    !0 === g.series.bandwidth.debug.active && (j = e);
                    if ("string" === typeof e.bandwidth.lineWidth
                    ) e.bandwidth.barWidth = parseInt(e.bandwidth.lineWidth, 0), e.nearBy.width = e.bandwidth.barWidth;
                    else {
                        var c = e.xaxis.p2c(e.xaxis.min + e.bandwidth.lineWidth) - e.xaxis.p2c(e.xaxis.min);
                        e.bandwidth.barWidth = c;
                        e.nearBy.width = e.bandwidth.lineWidth
                    }
                    k = a.getPlotOffset();
                    for (a = 0; a < e.data.length; a++) f(d, e, a, e.color)
                }
            }

            function f(a, d, g, e) {
                var c, j;
                j = g.length ? d.data[g[0]] : d.data[g];
                g = k.left + d.xaxis.p2c(j[0]);
                c = k.top + d.yaxis.p2c(j[1]);
                j = k.top + d.yaxis.p2c(j[2]);
                d.bandwidth.drawBandwidth(a, d.bandwidth, g, c, j, e)
            }

            function d(a, d, e, c) {
                var j = null;
                if (g.series.justEditing)
                    g.series.justEditing[1].seriesIndex === e &&
                    (a = null, j = a = [e, g.series.justEditing[1].dataIndex]);
                else if (g.grid.editable) {
                    if (j = null, c.bandwidth.show)
                        for (var b = 0; b < c.data.length; b++) {
                            var f, s, z;
                            z = c.data[b];
                            f = c.xaxis.p2c(z[0]) -
                                c.bandwidth.barWidth / 2;
                            s = c.yaxis.p2c(z[1]) - c.bandwidth.barWidth / 2;
                            z = c.yaxis.p2c(z[2]) - c.bandwidth.barWidth / 2;
                            q(a, f, f + c.bandwidth.barWidth) &&
                            (q(d, s, z) && (j = [e, b], c.editMode = "x", c.nearBy.findMode = "horizontal"),
                                q(d, s, s + c.bandwidth.barWidth) &&
                                (j = [e, [b, 1]], c
                                    .editMode = "y", c.nearBy
                                    .findMode = "vertical"),
                                q(d, z, z + c.bandwidth.barWidth) &&
                                (j = [e, [b, 2]], c.editMode = "y", c.nearBy.findMode = "vertical"))
                        }
                } else if (j = null, c.bandwidth.show)
                    for (b = 0; b < c.data.length; b++)
                        z = c.data[b], f = c.xaxis.p2c(z[0]) - c.bandwidth.barWidth / 2,
                            s = c.yaxis.p2c(z[1]), z = c.yaxis
                                .p2c(z[2]), q(a, f, f + c.bandwidth.barWidth) && q(d, s, z) && (j = [e, b]);
                return j
            }

            function e(a, d, c) {
                a.save();
                a.translate(-k.left, -k.top);
                f(a, d, c, "rgba(255,255,255," + d.bandwidth.highlight.opacity + ")");
                a.restore()
            }

            var k = null, g = null, j = null;
            n.hooks.processOptions.push(function(d, e) {
                e.series.bandwidth.active &&
                (c.extend(!0, e, x), c.plot.JUMlib.data.extendEmpty(e, t), g = e, d.hooks.processRawData.push(a), d
                    .hooks.drawSeries.push(h), !0 === g.series.bandwidth.debug.active &&
                (g.series.bandwidth.debug.createDocuTemplate =
                    b))
            })
        },
        options: m,
        name: p,
        version: "0.5"
    })
})(jQuery);
(function(c) {
    var p = "bubbles",
        m = {
            series: {
                bubbles: {
                    active: !1,
                    show: !1,
                    fill: !0,
                    lineWidth: 2,
                    highlight: { opacity: 0.5 },
                    drawbubble: function(c, q, n, a, b, h, f) {
                        c.fillStyle = f;
                        c.strokeStyle = f;
                        c.lineWidth = q.bubbles.lineWidth;
                        c.beginPath();
                        c.arc(n, a, h, 0, 2 * Math.PI, !0);
                        c.closePath();
                        q.bubbles.fill ? c.fill() : c.stroke();
                        q.bubbles.bubblelabel.show &&
                        (c.fillStyle = q.bubbles.bubblelabel.fillStyle, q = c.measureText(b), c
                            .fillText(b, n - q.width / 2, a + 4))
                    },
                    bubblelabel: { show: !1, fillStyle: "black" },
                    debug: { active: !1, createDocuTemplate: null }
                }
            }
        },
        x = { series: { editMode: "xy", nearBy: { distance: 6, findMode: "circle" } } };
    c.plot.plugins.push({
        init: function(t) {
            function q() {}

            function n() {
                var a, e;
                a = c.plot.JUMExample.docuObjectToTemplate([
                        { name: "data", tree: f.data }, {
                            name: "options.series.bubbles",
                            tree: m.series.bubbles,
                            takeDefault: !0
                        },
                        { name: "options.series.bubbles", tree: h.series.bubbles },
                        { name: "options.series.editMode", tree: m.series.editMode, takeDefault: !0 },
                        { name: "options.series.editMode", tree: h.series.editMode }, {
                            name: "options.series.nearBy",
                            tree: m.series.nearBy,
                            takeDefault: !0
                        }, { name: "options.series.nearBy", tree: h.series.nearBy }
                    ],
                    p);
                c.plot.JUMExample.extendDocuObject(a, p);
                e = c.plot.JUMExample.docuObjectToEdit(a, "");
                return{ data: a, form: e }
            }

            function a(a, c, k) {
                if (k.bubbles.show) {
                    !0 === h.series.bubbles.debug.active && (f = k);
                    b = a.getPlotOffset();
                    for (a = 0; a < k.data.length; a++) {
                        var g = c,
                            j = k,
                            l = k.data[a],
                            w = k.color,
                            u = void 0,
                            r = void 0,
                            v = void 0,
                            B = void 0,
                            u = b.left + j.xaxis.p2c(l[0]),
                            r = b.top + j.yaxis.p2c(l[1]),
                            B = l[2],
                            v = parseInt(j.yaxis.scale * l[2] / 2, 0);
                        j.bubbles.drawbubble(g,
                            j,
                            u,
                            r,
                            B,
                            v,
                            w,
                            void 0)
                    }
                }
            }

            var b = null, h = null, f = null;
            t.hooks.processOptions.push(function(d, e) {
                e.series.bubbles.active &&
                (c.plot.JUMlib.data.extendEmpty(e, x), h = e, d.hooks.processRawData.push(q), d.hooks.drawSeries
                    .push(a), !0 === h.series.bubbles.debug.active && (h.series.bubbles.debug.createDocuTemplate = n))
            })
        },
        options: m,
        name: p,
        version: "0.3"
    })
})(jQuery);
(function(c) {
    var p = "candlestick",
        m = {
            series: {
                candlestick: {
                    active: !1,
                    show: !1,
                    rangeWidth: 4,
                    rangeColor: "rgb(0,128,128)",
                    upColor: "rgb(255,0,0)",
                    downColor: "rgb(0,255,0)",
                    neutralColor: "rgb(0,0,0)",
                    lineWidth: "8px",
                    highlight: { opacity: 0.5 },
                    drawCandlestick: function(c, n, a, b) {
                        if (!0 === b) {
                            b = "rgba(255,255,255," + n.candlestick.highlight.opacity + ")";
                            var h, f;
                            h = n.xaxis.p2c(a[0] - n.candlestick.barWidth / 2);
                            f = n.yaxis.p2c(a[3]);
                            a = n.yaxis.p2c(a[4]);
                            c.beginPath();
                            c.strokeStyle = b;
                            c.lineWidth = n.candlestick.barWidth;
                            c.moveTo(h,
                                f);
                            c.lineTo(h, a)
                        } else {
                            b = n.xaxis.p2c(a[0]);
                            h = n.yaxis.p2c(a[3]);
                            f = n.yaxis.p2c(a[4]);
                            c.lineWidth = n.candlestick.rangeWidth;
                            c.beginPath();
                            c.strokeStyle = n.candlestick.rangeColor;
                            c.moveTo(b, h);
                            c.lineTo(b, f);
                            c.stroke();
                            var d;
                            b = n.xaxis.p2c(a[0] - n.candlestick.barWidth / 2);
                            h = n.yaxis.p2c(a[1]);
                            f = n.yaxis.p2c(a[2]);
                            d = a[1] > a[2] ? n.candlestick.upColor : n.candlestick.downColor;
                            a[1] == a[2] && (d = n.candlestick.neutralColor, f = h + 2);
                            c.beginPath();
                            c.strokeStyle = d;
                            c.lineWidth = n.candlestick.barWidth;
                            c.moveTo(b, h);
                            c.lineTo(b, f)
                        }
                        c.stroke()
                    },
                    debug: { active: !1, createDocuTemplate: null }
                }
            }
        },
        x = { series: { lines: { show: !1 } }, legend: { show: !1 } },
        t = c.plot.JUMlib.library.between;
    c.plot.candlestick = {};
    c.plot.candlestick.createCandlestick = function(c) {
        for (var n = [], a = [], b = 0;
            b < c.data.length;
            b++
        ) n.push([c.data[b][0], c.data[b][3]]), a.push([c.data[b][0], c.data[b][4]]);
        return[
            c, { label: "Max", data: a, lines: { show: !1 }, candlestick: { show: !1 }, nearBy: { findItem: null } },
            { label: "Min", data: n, lines: { show: !1 }, candlestick: { show: !1 }, nearBy: { findItem: null } }
        ]
    };
    c.plot.plugins.push({
        init: function(q) {
            function n(a,
                d) {
                !0 === d.candlestick.show && (d.nearBy.findItem = h, d.nearBy.drawHover = f)
            }

            function a() {
                var a, d;
                a = c.plot.JUMExample.docuObjectToTemplate([
                        { name: "data", tree: k.data }, {
                            name: "options.series.candlestick",
                            tree: m.series.candlestick,
                            takeDefault: !0
                        }, { name: "options.series.candlestick", tree: e.series.candlestick },
                        { name: "options.series.editMode", tree: e.series.editMode },
                        { name: "options.series.nearBy", tree: e.series.nearBy }
                    ],
                    p);
                c.plot.JUMExample.extendDocuObject(a, p);
                d = c.plot.JUMExample.docuObjectToEdit(a, "");
                return{
                    data: a,
                    form: d
                }
            }

            function b(a, c, b) {
                if (!0 === b.candlestick.show) {
                    !0 === e.series.candlestick.debug.active && (k = b);
                    if ("string" === typeof b.candlestick.lineWidth
                    )
                        b.candlestick.barWidth = parseInt(b.candlestick
                            .lineWidth,
                            0), b.nearBy.width = b.candlestick.barWidth;
                    else {
                        var f = b.xaxis.p2c(b.xaxis.min + b.candlestick.lineWidth) - b.xaxis.p2c(b.xaxis.min);
                        b.candlestick.barWidth = f;
                        b.nearBy.width = b.candlestick.lineWidth
                    }
                    d = a.getPlotOffset();
                    c.save();
                    c.translate(d.left, d.top);
                    for (a = 0; a < b.data.length; a++)
                        b.candlestick.drawCandlestick(c,
                            b,
                            b.data[a],
                            !1);
                    c.restore()
                }
            }

            function h(a, d, c, b) {
                var f = null;
                if (!0 === b.candlestick.show)
                    if (e.series.justEditing)
                        e.series.justEditing[1].seriesIndex === c &&
                        (a = null, f = a = [c, e.series.justEditing[1].dataIndex]);
                    else if (e.grid.editable) {
                        if (f = null, !0 === b.candlestick.show)
                            for (var r = 0; r < b.data.length; r++) {
                                var v, h, A, s, z;
                                z = b.data[r];
                                v = b.xaxis.p2c(z[0]) - b.candlestick.barWidth / 2;
                                h = b.yaxis.p2c(z[1]) - b.candlestick.rangeWidth / 2;
                                A = b.yaxis.p2c(z[2]) - b.candlestick.rangeWidth / 2;
                                s = b.yaxis.p2c(z[3]) -
                                    b.candlestick.rangeWidth /
                                    2;
                                z = b.yaxis.p2c(z[4]) - b.candlestick.rangeWidth / 2;
                                t(a, v, v + b.candlestick.barWidth) &&
                                (t(d, s, z) && (f = [c, r], b.editMode = "x", b.nearBy.findMode = "horizontal"),
                                    t(d, h, h + b.candlestick.rangeWidth) &&
                                    (f = [c, [r, 1]], b
                                        .editMode = "y", b.nearBy
                                        .findMode = "vertical"),
                                    t(d, A, A + b.candlestick.rangeWidth) &&
                                    (f = [c, [r, 2]], b
                                        .editMode = "y", b.nearBy
                                        .findMode = "vertical"),
                                    t(d, s, s + b.candlestick.rangeWidth) &&
                                    (f = [c, [r, 3]], b
                                        .editMode = "y", b.nearBy
                                        .findMode = "vertical"), t(d, z, z + b.candlestick.rangeWidth) &&
                                    (f = [c, [r, 4]], b.editMode = "y", b.nearBy.findMode =
                                        "vertical"))
                            }
                    } else {
                        f = null;
                        for (r = 0;
                            r < b.data.length;
                            r++
                        )
                            A = b.data[r], v = b.xaxis.p2c(A[0]) - b.candlestick.barWidth / 2, h = b.yaxis
                                .p2c(A[3]), A = b.yaxis
                                .p2c(A[4]), t(a, v, v + b.candlestick.barWidth) && t(d, h, A) && (f = [c, r])
                    }
                return f
            }

            function f(a, d, c) {
                a.save();
                d.candlestick.drawCandlestick(a, d, c.length ? d.data[c[0]] : d.data[c], !0);
                a.restore()
            }

            var d = null, e = null, k = null;
            q.hooks.processOptions.push(function(d, f) {
                f.series.candlestick.active &&
                (c.extend(!0, f, x), e = f, d.hooks.processRawData.push(n), d.hooks.drawSeries.push(b), !0 ===
                    e.series.candlestick.debug.active &&
                    (e.series.candlestick.debug.createDocuTemplate = a))
            })
        },
        options: m,
        name: p,
        version: "0.3"
    })
})(jQuery);
(function(c) {
    function p(a, c, h, f, d, e, k, g) {
        !1 === g &&
        (a.beginPath(), a.lineWidth = c.gantt.barheight, a.strokeStyle = "rgb(0,0,0)", a.moveTo(f, d), a.lineTo(e, d), a
            .stroke());
        a.beginPath();
        a.strokeStyle = k;
        a.lineWidth = c.gantt.barheight - 2;
        a.lineCap = "butt";
        a.moveTo(f + 1, d);
        a.lineTo(e - 1, d);
        a.stroke()
    }

    var m = "gantt",
        x = {
            series: {
                gantt: {
                    active: !1,
                    show: !1,
                    connectSteps: { show: !1, lineWidth: 2, color: "rgb(0,0,0)" },
                    barHeight: 0.6,
                    highlight: { opacity: 0.5 },
                    drawstep: p,
                    debug: { active: !1, createDocuTemplate: null }
                }
            }
        },
        t = { series: { lines: { show: !1 } } },
        q = { series: { editMode: "y", nearBy: { distance: 6, findItem: null, findMode: "circle", drawHover: null } } },
        n = c.plot.JUMlib.library.between;
    c.plot.plugins.push({
        init: function(a) {
            function b() {
                var a, d;
                a = c.plot.JUMExample.docuObjectToTemplate([
                        { name: "data", tree: l.data }, {
                            name: "options.series.gantt",
                            tree: x.series.gantt,
                            takeDefault: !0
                        },
                        { name: "options.series.gantt", tree: j.series.gantt },
                        { name: "options.series.editMode", tree: j.series.editMode },
                        { name: "options.series.nearBy", tree: j.series.nearBy }
                    ],
                    m);
                c.plot.JUMExample.extendDocuObject(a,
                    m);
                d = c.plot.JUMExample.docuObjectToEdit(a, "");
                return{ data: a, form: d }
            }

            function h(a, d) { !0 === d.gantt.show && (d.nearBy.findItem = e, d.nearBy.drawHover = k) }

            function f(a, b) {
                var e;
                w = a.getCanvas();
                c(w).parent();
                u = a.getAxes();
                g = a.getPlotOffset();
                r = a.getData();
                for (var f = 0; f < r.length; f++)
                    if (e = r[f], e.gantt.barheight = u.yaxis
                        .p2c(1) /
                        (u.yaxis.max - u.yaxis.min) *
                        e.gantt.barHeight, e.gantt.show) {
                        l = e;
                        for (var j = 0; j < e.data.length; j++) d(b, e, e.data[j], e.color, !1);
                        if (e.gantt.connectSteps.show)
                            for (var j = b, h = 0; h < e.data.length; h++)
                                for (var y =
                                        0;
                                    y < e.data.length;
                                    y++)
                                    if (e.data[h][2] == e.data[y][0]) {
                                        var E = g.left + u.xaxis.p2c(e.data[h][2]),
                                            k = g.top + u.yaxis.p2c(e.data[h][1]),
                                            C = g.top + u.yaxis.p2c(e.data[y][1]),
                                            n = j,
                                            m = e.gantt.connectSteps.lineWidth,
                                            q = e.gantt.connectSteps.color;
                                        n.beginPath();
                                        n.lineWidth = m;
                                        n.strokeStyle = q;
                                        n.moveTo(E, k);
                                        n.lineTo(E, C);
                                        n.stroke()
                                    }
                    }
            }

            function d(d, c, b, e, f) {
                var j, h, l;
                j = g.left + u.xaxis.p2c(b[0]);
                j = Math.min(Math.max(g.left, j), g.left + a.width());
                h = g.top + u.yaxis.p2c(b[1]);
                l = g.left + u.xaxis.p2c(b[2]);
                l = Math.min(Math.max(l, g.left),
                    g.left + a.width());
                if (l > g.left || j > g.left)
                    if (j < g.left + a.width() || l < g.left + a.width()
                    ) 4 === b.length ? p(d, c, b, j, h, l, e, f) : c.gantt.drawstep(d, c, b, j, h, l, e, f)
            }

            function e(a, d, c, b) {
                var e = null;
                if (j.series.justEditing)
                    j.series.justEditing[1].seriesIndex === c &&
                    (a = null, e = a = [c, j.series.justEditing[1].dataIndex]);
                else if (j.grid.editable) {
                    if (e = null, b.gantt.show)
                        for (var f = 0; f < b.data.length; f++) {
                            var g = b.data[f], l = b.xaxis.p2c(g[0]), h = b.xaxis.p2c(g[2]);
                            Math.abs(b.yaxis.p2c(g[1]) - d) <= b.gantt.barheight / 2 &&
                            (n(a, l, h) &&
                            (e = [
                                c,
                                f
                            ], b.editMode = "y", b.nearBy
                                .findMode = "vertical", b.nearBy
                                .width = g[2] - g[0]), n(a, l, l + b.nearBy.distance) &&
                            (e = [c, [f, 1]], b
                                .editMode = "x", b.nearBy
                                .findMode = "horizontal"), n(a, h, h + b.nearBy.distance) &&
                            (e = [c, [f, 2]], b.editMode = "x", b.nearBy.findMode = "horizontal"))
                        }
                } else if (e = null, b.gantt.show)
                    for (f = 0; f < b.data.length; f++)
                        g = b.data[f], l = b.xaxis.p2c(g[0]), h = b.xaxis
                            .p2c(g[2]), Math.abs(b.yaxis.p2c(g[1]) - d) <= b.gantt.barheight / 2 &&
                            n(a, l, h) &&
                            (e = [c, f]);
                return e
            }

            function k(a, c, b) {
                a.save();
                a.translate(-g.left, -g.top);
                d(a,
                    c,
                    b.length ? c.data[b[0]] : c.data[b],
                    "rgba(255,255,255, " + c.gantt.highlight.opacity + ")",
                    !0);
                a.restore()
            }

            var g = null, j = null, l = null, w, u, r;
            a.hooks.processOptions.push(function(a, d) {
                d.series.gantt.active &&
                (c.extend(!0, d, t), c.plot.JUMlib.data.extendEmpty(d, q), j = d, a.hooks.processRawData.push(h), a
                    .hooks.draw.push(f), !0 === j.series.gantt.debug.active &&
                (j.series.gantt.debug.createDocuTemplate = b))
            })
        },
        options: x,
        name: m,
        version: "0.3"
    })
})(jQuery);
(function(c) {
    var p = "grow",
        m = {
            series: {
                grow: {
                    active: !1,
                    stepDelay: 20,
                    steps: 100,
                    growings: [{ valueIndex: 1, stepMode: "linear", stepDirection: "up" }],
                    debug: { active: !1, createDocuTemplate: null }
                }
            }
        };
    c.plot.plugins.push({
        init: function(x) {
            function t() {
                var a, d;
                a = c.plot.JUMExample.docuObjectToTemplate([
                        { name: "data", tree: k.data }, {
                            name: "options.series.grow",
                            tree: m.series.grow,
                            takeDefault: !0
                        },
                        { name: "options.series.grow", tree: e.series.grow },
                        { name: "options.series.editMode", tree: m.series.editMode, takeDefault: !0 }, {
                            name: "options.series.editMode",
                            tree: e.series.editMode
                        }, { name: "options.series.nearBy", tree: m.series.nearBy, takeDefault: !0 },
                        { name: "options.series.nearBy", tree: e.series.nearBy }
                    ],
                    p);
                c.plot.JUMExample.extendDocuObject(a, p);
                d = c.plot.JUMExample.docuObjectToEdit(a, "");
                return{ data: a, form: d }
            }

            function q() {
                var a, c = new Date;
                if (d.actualStep < e.series.grow.steps) {
                    d.actualStep++;
                    for (var b = 0; b < d.length; b++)
                        for (var k = 0; k < d[b].grow.growings.length; k++)
                            if (a = d[b].grow
                                .growings[k], "function" === typeof a.stepMode) a.stepMode(d[b], d.actualStep, a);
                            else if ("linear" ===
                                a.stepMode) {
                                if (d
                                    .actualStep <=
                                    d[b].grow.steps)
                                    for (var r = 0;
                                        r < d[b].data.length;
                                        r++
                                    )
                                        "up" === a.stepDirection
                                            ? d[b].data[r][a
                                                .valueIndex] =
                                            d[b].dataOrg[r][a.valueIndex] / d[b].grow.steps * d.actualStep
                                            : "down" === a.stepDirection &&
                                            (d[b].data[r][a
                                                    .valueIndex] =
                                                d[b].dataOrg[r][a.valueIndex] +
                                                (d[b].yaxis.max - d[b].dataOrg[r][a.valueIndex]) /
                                                d[b].grow.steps *
                                                (d[b].grow.steps - d.actualStep))
                            } else if ("maximum" === a.stepMode) {
                                if (d.actualStep <= d[b].grow.steps)
                                    for (r = 0; r < d[b].data.length; r++)
                                        "up" === a.stepDirection
                                            ? d[b].data[r][a.valueIndex] =
                                            Math.min(d[b].dataOrg[r][a.valueIndex],
                                                d[b].yaxis.max / d[b].grow.steps * d.actualStep)
                                            : "down" === a.stepDirection &&
                                            (d[b].data[r][a.valueIndex] = Math
                                                .max(d[b].dataOrg[r][a.valueIndex],
                                                    d[b].yaxis
                                                    .max /
                                                    d[b].grow.steps *
                                                    (d[b].grow.steps - d.actualStep)))
                            } else if ("delay" === a.stepMode) {
                                if (d
                                    .actualStep ==
                                    d[b].grow.steps)
                                    for (r = 0;
                                        r < d[b].data.length;
                                        r++
                                    ) d[b].data[r][a.valueIndex] = d[b].dataOrg[r][a.valueIndex]
                            } else if (1 === d.actualStep)
                                for (r = 0; r < d[b].data.length; r++) d[b].data[r][g] = d[b].dataOrg[r][a.valueIndex];
                    f.setData(d);
                    f.draw();
                    a = new Date - c;
                    h = window.setTimeout(q, Math.max(0, e.series.grow.stepDelay - a))
                } else window.clearTimeout(h), h = null
            }

            function n() { h && (window.clearTimeout(h), h = null) }

            function a(b) {
                if (null === b || "object" !== typeof b) return b;
                var d = new b.constructor, c;
                for (c in b) d[c] = a(b[c]);
                return d
            }

            var b = !1, h, f = x, d = null, e = null, k = null, g;
            x.hooks.bindEvents.push(function(a) {
                e = a.getOptions();
                if (!0 === e.series.grow.active) {
                    for (var b = a
                                 .getData(),
                        f = 0;
                        f < d.length;
                        f++) e.series.grow.steps = Math.max(e.series.grow.steps, b[f].grow.steps);
                    0 === e.series.grow.stepDelay && e.series.grow.stepDelay++;
                    q();
                    a: {
                        for (var b = c.plot.plugins, f = 0, g = b.length; f < g; f++)
                            if ("resize" === b[f].name) {
                                b = !0;
                                break a
                            }
                        b = !1
                    }
                    b && a.getPlaceholder().bind("resize", n)
                }
            });
            x.hooks.drawSeries.push(function(c, f, h) {
                e = c.getOptions();
                g = e.series.grow.valueIndex;
                if (!0 === e.series.grow.active &&
                (!0 === e.series.grow.debug.active && (k = h, e.series.grow.debug.createDocuTemplate = t), !1 === b)) {
                    d = c.getData();
                    d.actualStep = 0;
                    for (f = d.growingIndex = 0; f < d.length; f++) {
                        d[f].dataOrg = a(d[f].data);
                        for (h = 0;
                            h <
                                d[f].data.length;
                            h++) d[f].data[h][g] = 0
                    }
                    c.setData(d);
                    b = !0
                }
            })
        },
        options: m,
        name: p,
        version: "0.4"
    })
})(jQuery);
(function(c) {
    var p = "pyramid",
        m = {
            series: {
                pyramid: {
                    active: !1,
                    show: !1,
                    mode: "pyramid",
                    fill: !0,
                    debug: { active: !1, createDocuTemplate: null },
                    highlight: { opacity: 0.5 },
                    label: { show: !1, align: "center", font: "20px Times New Roman", fillStyle: "Black" }
                }
            }
        },
        x = { series: { nearBy: { distance: 6, findItem: null, findMode: "circle", drawHover: null } } },
        t = { grid: { show: !1 } },
        q = c.plot.JUMlib.library.between;
    c.plot.plugins.push({
        init: function(n) {
            function a(a, b) { !0 === b.pyramid.show && (b.nearBy.findItem = e, b.nearBy.drawHover = k) }

            function b() {
                var a,
                    b;
                a = c.plot.JUMExample.docuObjectToTemplate([
                        { name: "data", tree: l.data }, {
                            name: "options.series.pyramid",
                            tree: m.series.pyramid,
                            takeDefault: !0
                        }, { name: "options.series.pyramid", tree: g.series.pyramid },
                        { name: "options.series.editMode", tree: m.series.editMode, takeDefault: !0 },
                        { name: "options.series.editMode", tree: g.series.editMode },
                        { name: "options.series.nearBy", tree: m.series.nearBy, takeDefault: !0 },
                        { name: "options.series.nearBy", tree: g.series.nearBy }
                    ],
                    p);
                c.plot.JUMExample.extendDocuObject(a, p);
                b = c.plot.JUMExample.docuObjectToEdit(a,
                    "");
                return{ data: a, form: b }
            }

            function h(a, b, c) {
                if (c.pyramid.show) {
                    !0 === g.series.pyramid.debug.active && (l = c);
                    j = a.getPlotOffset();
                    u = c.data[0].value;
                    v = b.canvas.height;
                    B = b.canvas.width;
                    w = b.canvas.height / c.data.length;
                    r = b.canvas.width / 2;
                    for (a = 0; a < c.data.length; a++) f(b, c, a, g.colors[a])
                }
            }

            function f(a, b, e, f) {
                var g, h, j;
                g = b.data[e].value * a.canvas.width / u;
                j = a.canvas.height - w * e;
                h = e + 1 == b.data.length ? 0 : b.data[e + 1].value * a.canvas.width / u;
                if (c.isFunction(b.pyramid.mode)) b.pyramid.mode(a, b, r, j, g, w, h, f);
                else
                    switch (b.pyramid.mode) {
                    case "pyramid":
                        d(a,
                            b,
                            j,
                            g,
                            w,
                            h,
                            f);
                        break;
                    case "slice":
                        h = w;
                        var l = j - h / 2;
                        a.save();
                        a.beginPath();
                        a.lineWidth = 1;
                        a.fillStyle = f;
                        a.strokeStyle = f;
                        a.translate(r - g / 2, l - h / 2);
                        a.scale(g / 2, h / 2);
                        a.arc(1, 1, 1, 0, 2 * Math.PI, !1);
                        a.closePath();
                        a.fill();
                        a.restore();
                        break;
                    default:
                        d(a, b, j, g, w, h, f)
                    }
                if (!0 === b.pyramid.label.show) {
                    e = b.data[e];
                    j -= w / 2;
                    a.font = b.pyramid.label.font;
                    a.fillStyle = b.pyramid.label.fillStyle;
                    f = a.measureText(e.label);
                    switch (b.pyramid.label.align) {
                    case "center":
                        b = a.canvas.width / 2 - f.width / 2;
                        break;
                    case "left":
                        b = 0;
                        break;
                    case "right":
                        b =
                            a.canvas.width - f.width;
                        break;
                    default:
                        b = a.canvas.width - f.width
                    }
                    a.fillText(e.label, b, j)
                }
            }

            function d(a, b, c, d, e, f, g) {
                a.beginPath();
                a.lineWidth = 1;
                a.fillStyle = g;
                a.strokeStyle = g;
                a.moveTo(r - d / 2, c);
                a.lineTo(r + d / 2, c);
                a.lineTo(r + f / 2, c - e);
                a.lineTo(r - f / 2, c - e);
                a.closePath();
                a.fill()
            }

            function e(a, b, c, d) {
                var e = null;
                b = Math.floor((v - b) / w);
                q(b, 0, d.data.length - 1) &&
                (d = d.data[b].value * B / u, !0 === q(a, r - d / 2, r + d / 2) && (e = [c, b]));
                return e
            }

            function k(a, b, c) {
                a.save();
                a.translate(-j.left, -j.top);
                f(a,
                    b,
                    c,
                    "rgba(255,255,255," +
                    b.pyramid.highlight.opacity +
                    ")");
                a.restore()
            }

            var g = null, j = null, l = null, w = null, u, r, v, B;
            n.hooks.processOptions.push(function(d, e) {
                !0 === e.series.pyramid.active &&
                (c.extend(!0, e, t), c.plot.JUMlib.data.extendEmpty(e, x), g = e, d.hooks.processRawData.push(a), d
                    .hooks.drawSeries.push(h), !0 === g.series.pyramid.debug.active &&
                (g.series.pyramid.debug.createDocuTemplate = b))
            })
        },
        options: m,
        name: p,
        version: "0.2"
    })
})(jQuery);
(function(c) {
    var p = "radar",
        m = {
            series: {
                radar: {
                    active: !1,
                    show: !1,
                    radarSize: 0.8,
                    delay: 10,
                    angleStep: 1,
                    angleSize: 10,
                    angleSteps: 6,
                    color: "darkgreen",
                    backColor: "darkgreen",
                    debug: { active: !1, createDocuTemplate: null }
                }
            }
        },
        x = { grid: { show: !1 } };
    c.plot.plugins.push({
        init: function(t) {
            function q() {
                var a, b;
                a = c.plot.JUMExample.docuObjectToTemplate([
                        { name: "data", tree: g.data }, {
                            name: "options.series.radar",
                            tree: m.series.radar,
                            takeDefault: !0
                        },
                        { name: "options.series.radar", tree: k.series.radar }
                    ],
                    p);
                c.plot.JUMExample.extendDocuObject(a,
                    p);
                b = c.plot.JUMExample.docuObjectToEdit(a, "");
                return{ data: a, form: b }
            }

            function n() {}

            function a(a, h) {
                j = a.getData();
                k = a.getOptions();
                h.clearRect(0, 0, h.canvas.width, h.canvas.height);
                f = Math.min(h.canvas.width, h.canvas.height) / 2 * k.series.radar.radarSize;
                d = e = h.canvas.height / 2;
                h.beginPath();
                h.lineWidth = 2;
                h.strokeStyle = k.series.radar.color;
                h.fillStyle = k.series.radar.backColor;
                h.arc(e, d, f, 0, 2 * Math.PI, !0);
                h.closePath();
                h.fill();
                l = document.createElement("canvas");
                l.width = h.canvas.width;
                l.height = h.canvas.height;
                c(l).css({ position: "absolute", left: 0, top: 0 });
                c(l).appendTo(t.getPlaceholder());
                w = l.getContext("2d");
                for (var u = 0; u < j.length; u++) {
                    var n = h, s = j[u];
                    !0 === k.series.radar.debug.active && (g = s);
                    for (var z = 0; z < s.data.length; z++) {
                        var D = n,
                            y = s,
                            E = s.data[z],
                            F = 2 * Math.PI * E[0] / 360,
                            C = y.radar.itemSize,
                            m = d + Math.round(Math.cos(F) * f * E[1] / 100) - C,
                            E = e + Math.round(Math.sin(F) * f * E[1] / 100) - C;
                        D.beginPath();
                        D.lineWidth = 1;
                        D.fillStyle = y.color;
                        D.strokeStyle = y.color;
                        D.arc(m, E, C, 0, 2 * Math.PI, !0);
                        D.closePath();
                        D.fill()
                    }
                }
                window.setInterval(b,
                    k.series.radar.delay)
            }

            function b() {
                var a = k.series.radar.angleSize, b = k.series.radar.angleSteps, c, d;
                w.clearRect(0, 0, w.canvas.width, w.canvas.height);
                for (var e = w.lineWidth = 1; e <= b; e++)
                    c = (b - e + 1) / 10, d = (e - 1) * a + u, h("rgba(255,255,255," + c + ")", d, a + d);
                h(k.series.radar.backColor, u + b * a, u);
                u += k.series.radar.angleStep;
                359 < u && (u = 0)
            }

            function h(a, b, c) {
                b = 2 * Math.PI * b / 360;
                c = 2 * Math.PI * c / 360;
                var g = d + Math.round(Math.cos(b) * f), h = e + Math.round(Math.sin(b) * f);
                w.strokeStyle = a;
                w.fillStyle = a;
                w.beginPath();
                w.moveTo(d, e);
                w.lineTo(g,
                    h);
                w.arc(d, e, f, b, c);
                w.lineTo(d, e);
                w.closePath();
                w.fill()
            }

            var f = null, d = null, e = null, k = null, g = null, j = null, l = null, w = null, u = 0;
            t.hooks.processOptions.push(function(b, d) {
                !0 === d.series.radar.active &&
                (c.extend(!0, d, x), k = d, b.hooks.processRawData.push(n), b.hooks.draw
                    .push(a), !0 === k.series.radar.debug.active && (k.series.radar.debug.createDocuTemplate = q))
            })
        },
        options: m,
        name: p,
        version: "0.1"
    })
})(jQuery);
(function(c) {
    var p = "rose",
        m = {
            series: {
                rose: {
                    active: !1,
                    show: !1,
                    roseSize: 0.7,
                    leafSize: 0.7,
                    dataMin: 0,
                    dataMax: 100,
                    drawGrid: { drawValue: !0, drawLabel: !0, labelPos: 0.5, gridMode: "data" },
                    highlight: { opacity: 0.5 },
                    debug: { active: !1, createDocuTemplate: null }
                }
            }
        },
        x = { grid: { show: !1 } },
        t = { series: { nearBy: { distance: 6, findItem: null, findMode: "circle", drawEdit: null, drawHover: null } }, grid: { ranges: 5, font: "18px Times New Roman" } },
        q = c.plot.JUMlib.library.between,
        n = c.plot.JUMlib.data.createColors,
        a = c.plot.JUMlib.data.getColor;
    c.plot.plugins.push({
        init: function(b) {
            function h() {
                var a,
                    b;
                a = c.plot.JUMExample.docuObjectToTemplate([
                        { name: "data", tree: w.data }, {
                            name: "options.series.rose",
                            tree: m.series.rose,
                            takeDefault: !0
                        },
                        { name: "options.series.rose", tree: l.series.rose }
                    ],
                    p);
                c.plot.JUMExample.extendDocuObject(a, p);
                b = c.plot.JUMExample.docuObjectToEdit(a, "");
                return{ data: a, form: b }
            }

            function f(b, c, f) {
                var g, h, j, l, r;
                if (f.rose.show) {
                    w = f;
                    for (var k = b = 0; k < f.data.length; k++)
                        r = f.data[k], g = b + z, h = b + s - z, b += s, r.length
                            ? (j = e(r[0]), l = {
                                ctx: c,
                                serie: f,
                                serieIndex: k,
                                colors: D,
                                radius: j,
                                left: B,
                                top:
                                    A
                            }, l = a(l), d(c,
                                r[1],
                                r[2],
                                j,
                                l))
                            : (j = e(r), l = { ctx: c, serie: f, serieIndex: k, colors: D, radius: j, left: B, top: A },
                                l = a(l), d(c, g, h, j, l))
                }
            }

            function d(a, b, c, d, e) {
                b = 2 * Math.PI * b / 360;
                c = 2 * Math.PI * c / 360;
                var f = B + Math.round(Math.cos(b) * d), g = A + Math.round(Math.sin(b) * d);
                a.strokeStyle = e;
                a.fillStyle = e;
                a.beginPath();
                a.moveTo(B, A);
                a.lineTo(f, g);
                a.arc(B, A, d, b, c);
                a.lineTo(B, A);
                a.closePath();
                a.fill()
            }

            function e(a) { return(a - l.series.rose.dataMin) / (l.series.rose.dataMax - l.series.rose.dataMin) * v }

            function k(a, b) {
                var c = 0, d;
                u = b;
                r = a.getData();
                b.strokeStyle =
                    l.grid.tickColor;
                b.fillStyle = l.grid.color;
                for (d = 1; d <= l.grid.ranges; d++) {
                    var e = b, f = d, g = void 0;
                    e.beginPath();
                    e.moveTo(B, A);
                    g = v / l.grid.ranges * f;
                    e.arc(B, A, g, 0, 2 * Math.PI);
                    e.closePath();
                    e.stroke();
                    !0 === l.series.rose.drawGrid.drawValue &&
                        b.fillText(l.series.rose.dataMin +
                            (l.series.rose.dataMax - l.series.rose.dataMin) / l.grid.ranges * d,
                            B + v / l.grid.ranges * d,
                            A - 1)
                }
                for (d = 0; d < l.grid.tickLabel.length; d++) {
                    e = b;
                    g = 2 * Math.PI * c / 360;
                    f = B + Math.round(Math.cos(g) * v);
                    g = A + Math.round(Math.sin(g) * v);
                    e.beginPath();
                    e.moveTo(B, A);
                    e.lineTo(f,
                        g);
                    e.closePath();
                    e.stroke();
                    e = b;
                    f = c + s * w.rose.drawGrid.labelPos;
                    g = l.grid.tickLabel[d];
                    e.font = l.grid.font;
                    var h = 2 * Math.PI * f / 360,
                        j = B + Math.round(Math.cos(h) * v),
                        h = A + Math.round(Math.sin(h) * v),
                        k = e.measureText(g);
                    180 > f && (h += 10);
                    q(f, 90, 270) && (j -= k.width);
                    e.fillText(g, j, h);
                    c += s
                }
            }

            function g(a, b, c, d) {
                var f;
                a: {
                    f = function(c, d) {
                        var e, f, g, h;
                        u.save();
                        u.beginPath();
                        e = 2 * Math.PI * c / 360;
                        f = 2 * Math.PI * (c + s) / 360;
                        g = B + Math.round(Math.cos(e) * d);
                        h = A + Math.round(Math.sin(e) * d);
                        u.moveTo(B, A);
                        u.lineTo(g, h);
                        u.arc(B, A, d, e, f);
                        u.closePath();
                        e = u.isPointInPath(a, b);
                        u.restore();
                        return e
                    };
                    var g = null, h, j, l;
                    for (l = h = 0; l < d.data.length; l++) {
                        j = f(h, v);
                        if (!0 === j) {
                            g = [c, l];
                            for (c = 0; c < r.length; c++) j = e(r[c].data[l]), j = f(h, j), !0 === j && (g = [c, l]);
                            f = g;
                            break a
                        }
                        h += s
                    }
                    f = g
                }
                return f
            }

            function j(a, b, c) {
                c *= s;
                d(a, c, c + s, v, "rgba(255,255,255," + b.rose.highlight.opacity + ")")
            }

            var l = null, w = null, u, r, v, B, A, s, z, D;
            b.hooks.processOptions.push(function(a, b) {
                !0 === b.series.rose.active &&
                (c.extend(!0, b, x), c.plot.JUMlib.data.extendEmpty(b, t), l = b, a.hooks.drawSeries.push(f), a.hooks
                        .draw.push(k),
                    !0 === l.series.rose.debug.active && (l.series.rose.debug.createDocuTemplate = h))
            });
            b.hooks.processRawData.push(function(a, b) {
                if (!0 === b.rose.show) {
                    var c = a.getCanvas();
                    v = Math.min(c.width, c.height) / 2 * l.series.rose.roseSize;
                    B = A = c.height / 2;
                    D = n(l, b.data[0].length);
                    b.nearBy.findItem = g;
                    b.nearBy.drawHover = j;
                    a.getPlotOffset();
                    switch (l.series.rose.drawGrid.gridMode) {
                    case "data":
                        s = 360 / b.data.length;
                        break;
                    case "ticks":
                        s = 360 / l.grid.tickLabel.length
                    }
                    z = s * (1 - l.series.rose.leafSize) / 2
                }
            })
        },
        options: m,
        name: p,
        version: "0.1"
    })
})(jQuery);
(function(c) {
    var p = "rectangle",
        m = {
            series: {
                rectangle: {
                    active: !1,
                    show: !1,
                    fill: !0,
                    lineWidth: 2,
                    directions: "tlbr",
                    highlight: { opacity: 0.5 },
                    drawRectangle: function(a, c, f) {
                        f = c.data[f];
                        a.save();
                        a.linewidth = c.rectangle.lineWidth;
                        !0 === c.rectangle.fill
                            ? (a.fillStyle = f.pos.color, a.fillRect(f.pos.x, f.pos.y, f.pos.w, f.pos.h))
                            : (a.strokeStyle = f.pos.color, a.strokeRect(f.pos.x, f.pos.y, f.pos.w, f.pos.h));
                        a.restore();
                        a.fillStyle = c.color;
                        a.strokeStyle = c.color;
                        a.lineWidth = c.rectangle.lineWidth;
                        c.rectangle.label.show &&
                        (a.fillStyle =
                            c.rectangle.label.fillStyle, c = c.xaxis.options
                            .font, a.font = c.style +
                            " " +
                            c.variant +
                            " " +
                            c.weight +
                            " " +
                            c.size +
                            "px '" +
                            c.family +
                            "'", c = a.measureText(f.label), a
                            .fillText(f.label, f.pos.x + f.pos.w / 2 - c.width / 2, f.pos.y + f.pos.h / 2))
                    },
                    label: { show: !1, fillStyle: "black" },
                    debug: { active: !1, createDocuTemplate: null }
                }
            }
        },
        x = { grid: { show: !1 }, xaxes: [{ min: 0, max: 100 }], yaxes: [{ min: 0, max: 100 }] },
        t = { series: { editMode: "none", nearBy: { distance: 6, findMode: "circle" } } },
        q = c.plot.JUMlib.library.between,
        n = c.plot.JUMlib.data.createFont,
        a = c.plot.JUMlib.data.createColors;
    c.plot.plugins.push({
        init: function(b) {
            function h(a, b) { !0 === b.rectangle.show && (b.nearBy.drawHover = e, b.nearBy.findItem = k) }

            function f() {
                var a, b;
                a = c.plot.JUMExample.docuObjectToTemplate([
                        { name: "data", tree: l.data }, {
                            name: "options.series.rectangle",
                            tree: m.series.rectangle,
                            takeDefault: !0
                        },
                        { name: "options.series.rectangle", tree: j.series.rectangle },
                        { name: "options.series.editMode", tree: m.series.editMode, takeDefault: !0 },
                        { name: "options.series.editMode", tree: j.series.editMode }, {
                            name: "options.series.nearBy",
                            tree: m.series.nearBy,
                            takeDefault: !0
                        }, { name: "options.series.nearBy", tree: j.series.nearBy }
                    ],
                    p);
                c.plot.JUMExample.extendDocuObject(a, p);
                b = c.plot.JUMExample.docuObjectToEdit(a, "");
                return{ data: a, form: b }
            }

            function d(b, d, e) {
                var f = 0, h = 0, k = 100, D = 100;
                e.xaxis.options.font || (e.xaxis.options.font = n(b.getPlaceholder()));
                if (e.rectangle.show) {
                    l = e;
                    g = b.getPlotOffset();
                    w = a(j, e.data.length);
                    for (b = u = 0; b < e.data.length; b++) u += e.data[b].data;
                    for (b = 0; b < e.data.length; b++) {
                        var y, E, m, C;
                        C = e.data[b];
                        switch (e.rectangle.directions[b % e.rectangle.directions.length]) {
                        case "t":
                            y =
                                h;
                            E = f;
                            m = k;
                            C = 100 * (100 * (C.data / u) / k);
                            f += C;
                            D -= C;
                            break;
                        case "b":
                            y = h;
                            C = 100 * (100 * (C.data / u) / k);
                            m = k;
                            E = f - C + D;
                            D -= C;
                            break;
                        case "l":
                            y = h;
                            E = f;
                            m = 100 * (100 * (C.data / u) / D);
                            C = D;
                            h += m;
                            k -= m;
                            break;
                        case "r":
                            E = f;
                            m = 100 * (100 * (C.data / u) / D);
                            y = h + k - m;
                            C = D;
                            k -= m;
                            break;
                        default:
                            y = h, E = f, m = k, C = 100 * (100 * (C.data / u) / k), f += C, D -= C
                        }
                        var q;
                        y = g.left + e.xaxis.p2c(y);
                        E = g.top + e.yaxis.p2c(100 - E);
                        m = e.xaxis.p2c(m) - e.xaxis.p2c(0);
                        C = e.yaxis.p2c(0) - e.yaxis.p2c(C);
                        q = c.plot.JUMlib.data.getColor({
                            ctx: d,
                            serie: l,
                            dataIndex: b,
                            colors: w,
                            left: y,
                            top: E,
                            height: C,
                            width: m
                        });
                        e.data[b].pos = { x: y, y: E, w: m, h: C, color: q };
                        e.rectangle.drawRectangle(d, e, b)
                    }
                }
            }

            function e(a, b, c) { b.rectangle.drawRectangle(a, b, c) }

            function k(a, b, c, d) {
                for (var e = null, f = 0; f < d.data.length; f++) {
                    var g = d.data[f].pos;
                    q(a, g.x, g.x + g.w) && q(b, g.y, g.y + g.h) && (e = [c, f])
                }
                return e
            }

            var g = null, j = null, l = null, w, u;
            b.hooks.processOptions.push(function(a, b) {
                b.series.rectangle.active &&
                (c.extend(!0, b, x), c.plot.JUMlib.data.extendEmpty(b, t), j = b, a.hooks.processRawData.push(h), a
                    .hooks.drawSeries.push(d), !0 === j.series.rectangle.debug.active &&
                (j.series.rectangle.debug.createDocuTemplate = f))
            })
        },
        options: m,
        name: p,
        version: "0.3"
    })
})(jQuery);
(function(c) {
    var p = "spider",
        m = {
            series: {
                spider: {
                    active: !1,
                    show: !1,
                    spiderSize: 0.8,
                    lineWidth: 3,
                    lineStyle: "rgba(0,0,0,0.5)",
                    pointSize: 6,
                    scaleMode: "leg",
                    legMin: null,
                    legMax: null,
                    connection: { width: 4 },
                    highlight: { opacity: 0.5, mode: "point" },
                    legs: { font: "20px Times New Roman", fillStyle: "Black", legScaleMin: 0.95, legScaleMax: 1.05, legStartAngle: 0 },
                    debug: { active: !1, createDocuTemplate: null }
                }
            }
        },
        x = {
            series: {
                editMode: "xy",
                nearBy: { distance: 6, findItem: null, findMode: "circle", drawEdit: null, drawHover: null }
            },
            grid: { mode: "radar" }
        },
        t = { grid: { show: !1, tickColor: "rgba(0,0,0,0.15)", ticks: 5 } },
        q = c.plot.JUMlib.library.between;
    c.plot.plugins.push({
        init: function(n) {
            function a(a, b) {
                !0 === b.spider.show && (b.nearBy.drawEdit = l, b.nearBy.findItem = j, b.nearBy.drawHover = w)
            }

            function b() {
                var a, b;
                a = c.plot.JUMExample.docuObjectToTemplate([
                        { name: "data", tree: A.data }, {
                            name: "options.series.spider",
                            tree: m.series.spider,
                            takeDefault: !0
                        },
                        { name: "options.series.spider", tree: v.series.spider },
                        { name: "options.series.editMode", tree: m.series.editMode, takeDefault: !0 },
                        { name: "options.series.editMode", tree: v.series.editMode },
                        { name: "options.series.nearBy", tree: m.series.nearBy, takeDefault: !0 },
                        { name: "options.series.nearBy", tree: v.series.nearBy }
                    ],
                    p);
                c.plot.JUMExample.extendDocuObject(a, p);
                b = c.plot.JUMlib.docu.docuObjectToEdit(a, "");
                return{ data: a, form: b }
            }

            function h(a, b) {
                s = a.getData();
                v = a.getOptions();
                !0 === v.series.spider.debug.active && (A = s[0]);
                b.clearRect(0, 0, b.canvas.width, b.canvas.height);
                u = Math.min(b.canvas.width, b.canvas.height) / 2 * s[0].spider.spiderSize;
                r = B = b.canvas.height /
                    2;
                var c = [], h;
                if ("leg" === s[0].spider.scaleMode) for (h = 0; h < s[0].data.length; h++) c.push(f(h));
                else {
                    var j;
                    h = Number.POSITIVE_INFINITY;
                    j = Number.NEGATIVE_INFINITY;
                    for (var l = 0;
                        l < s[0].data.length;
                        l++
                    )
                        for (var k = 0; k < s.length; k++)
                            h = Math.min(h, s[k].data[l][1]), j = Math.max(j, s[k].data[l][1]);
                    h *= s[0].spider.legs.legScaleMin;
                    j *= s[0].spider.legs.legScaleMax;
                    v.series.spider.legMin && (h = v.series.spider.legMin);
                    v.series.spider.legMax && (j = v.series.spider.legMax);
                    j = { min: h, max: j, range: j - h };
                    for (h = 0; h < s[0].data.length; h++) c.push(j)
                }
                s.ranges =
                    c;
                c = v.grid;
                h = function(a, b) {
                    a.lineWidth = 1;
                    a.strokeStyle = b.tickColor;
                    for (var c = 1; c <= b.ticks; c++)
                        a.beginPath(), a.arc(r, B, u / b.ticks * c, 0, 2 * Math.PI, !0), a.closePath(), a.stroke();
                    for (var d = c = null, e = 0;
                        e < p;
                        e++
                    ) null === c && (c = g(p, e, 100), d = g(p, Math.floor(p / 4), 100)), w(a, e), n(a, e, c, d)
                };
                var w = function(a, b) {
                        var c;
                        a.beginPath();
                        a.lineWidth = m.series.spider.lineWidth;
                        a.strokeStyle = m.series.spider.lineStyle;
                        a.moveTo(B, r);
                        c = g(p, b, 100);
                        a.lineTo(c.x, c.y);
                        a.stroke()
                    },
                    n = function(a, b, c, d) {
                        var e, f, h;
                        e = g(p, b, 100);
                        a.font = s[0].spider.legs.font;
                        a.fillStyle = s[0].spider.legs.fillStyle;
                        f = a.measureText(s[0].spider.legs.data[b].label);
                        h = e.y > c.y ? 15 : -15;
                        q(e.y, c.y + 10, c.y - 10) && (h = 0);
                        d = e.x < d.x ? -1 * f.width - f.width / 2 : 0;
                        q(e.x, c.x + 10, c.x - 10) && (d = f.width / 2);
                        a.fillText(s[0].spider.legs.data[b].label, e.x + d, e.y + h)
                    },
                    p = s[0].data.length;
                for (j = 0; j < s.length; j++) e(b, p, s[j], s[j].color);
                for (j = 0; j < s.length; j++) d(b, p, s[j], s[j].color);
                switch (c.mode) {
                case "radar":
                    h(b, c);
                    break;
                case "spider":
                    b.linewidth = 1;
                    b.strokeStyle = c.tickColor;
                    for (j = 0; j <= c.ticks; j++) {
                        l = g(p,
                            0,
                            100 /
                            c.ticks *
                            j);
                        b.beginPath();
                        b.moveTo(l.x, l.y);
                        for (h = 1; h < p; h++) l = g(p, h, 100 / c.ticks * j), b.lineTo(l.x, l.y);
                        b.closePath();
                        b.stroke()
                    }
                    j = c = null;
                    for (h = 0; h < p; h++)
                        null === c && (c = g(p, h, 100), j = g(p, Math.floor(p / 4), 100)), w(b, h), n(b, h, c, j);
                    break;
                default:
                    h(b, c)
                }
            }

            function f(a) {
                for (var b = Number
                             .POSITIVE_INFINITY,
                c = Number.NEGATIVE_INFINITY,
                d = 0;
                d < s.length;
                d++) b = Math.min(b, s[d].data[a][1]), c = Math.max(c, s[d].data[a][1]);
                b *= s[0].spider.legs.legScaleMin;
                c *= s[0].spider.legs.legScaleMax;
                v.series.spider.legMin && (b = v.series.spider.legMin);
                v.series.spider.legMax && (c = v.series.spider.legMax);
                return{ min: b, max: c, range: c - b }
            }

            function d(a, b, c, d) {
                for (var e = 0; e < c.data.length; e++) {
                    var f = a, h = b, j = c, l = e, r = d, u = void 0, u = k(j, s.ranges, l), u = g(h, l, u);
                    f.beginPath();
                    f.lineWidth = 1;
                    f.fillStyle = r;
                    f.strokeStyle = r;
                    f.arc(u.x, u.y, j.spider.pointSize, 0, 2 * Math.PI, !0);
                    f.closePath();
                    f.fill()
                }
            }

            function e(a, b, c, d, e) {
                var f;
                e || (e = !1);
                a.beginPath();
                a.lineWidth = c.spider.connection.width;
                a.strokeStyle = d;
                a.fillStyle = d;
                f = k(c, s.ranges, 0);
                f = g(b, 0, f);
                a.moveTo(f.x, f.y);
                for (d = 1;
                    d <
                        c.data.length;
                    d++) f = k(c, s.ranges, d), f = g(b, d, f), a.lineTo(f.x, f.y);
                f = k(c, s.ranges, 0);
                f = g(b, 0, f);
                a.lineTo(f.x, f.y);
                !0 === e ? a.fill() : !0 === c.spider.fill ? a.fill() : a.stroke()
            }

            function k(a, b, c, d) {
                d
                    ? a = 100 * ((d - b[c].min) / b[c].range)
                    : (a = Math.max(Math.min(a.data[c][1], b[c].max), b[c].min), a = 100 * ((a - b[c].min) / b[c].range));
                return a
            }

            function g(a, b, c) {
                var d, e;
                e = 2 * Math.PI * v.series.spider.legs.legStartAngle / 360;
                d = r + Math.round(Math.cos(2 * Math.PI / a * b + e) * u * c / 100);
                a = B + Math.round(Math.sin(2 * Math.PI / a * b + e) * u * c / 100);
                return{
                    x: d,
                    y: a
                }
            }

            function j(a, b, c, d) {
                var e = null;
                if (v.series.justEditing) {
                    if (v.series.justEditing[1].seriesIndex === c) {
                        var f, h, j, e = null;
                        h = d.data.length;
                        var l = v.series.justEditing[1].dataIndex;
                        f = (a - r) * (a - r) + (b - B) * (b - B);
                        f = Math.sqrt(f);
                        j = f = 100 * (f / u);
                        f = s.ranges[l];
                        f = f.min + f.range / 100 * j;
                        h = g(h, l, j);
                        a = Math.abs(h.x - a);
                        b = Math.abs(h.y - b);
                        Math.sqrt(a * a + b * b) <= d.spider.pointSize && (e = [c, l, f, 0])
                    }
                } else {
                    l = null;
                    f = d.data.length;
                    for (e = 0; e < f; e++)
                        j = g(f, e, k(d, s.ranges, e)), h = Math.abs(j.x - a), j = Math.abs(j.y - b), h = Math
                            .sqrt(h * h + j * j), h <= d.nearBy.distance &&
                        (l = [c, e]);
                    e = l
                }
                return e
            }

            function l(a, b, c, d) {
                a.beginPath();
                a.lineWidth = 1;
                b = "rgba(255, 0, 0, " + d.spider.highlight.opacity + ")";
                a.fillStyle = b;
                a.strokeStyle = b;
                b = k(d, s.ranges, v.series.justEditing[1].dataIndex, v.series.justEditing[0].value);
                d = g(d.data.length, v.series.justEditing[1].dataIndex, b);
                a.arc(d.x, d.y, v.series.spider.pointSize, 0, 2 * Math.PI, !0);
                a.closePath();
                a.fill()
            }

            function w(a, b) {
                if (!b.justEditing) {
                    var c = "rgba(255, 255, 255, " + b.spider.highlight.opacity + ")", f = b.data.length;
                    switch (b.spider.highlight.mode) {
                    case "point":
                        d(a,
                            f,
                            b,
                            c);
                        break;
                    case "line":
                        e(a, f, b, c, !1);
                        break;
                    case "area":
                        e(a, f, b, b.color, !0)
                    }
                }
            }

            var u = null, r = null, v = null, B = null, A = null, s;
            n.hooks.processOptions.push(function(d, e) {
                e.series.spider.active &&
                (c.extend(!0, e, t), c.plot.JUMlib.data.extendEmpty(e, x), v = e, d.hooks.processRawData.push(a), d
                    .hooks.draw.push(h), !0 === v.series.spider.debug.active &&
                (v.series.spider.debug.createDocuTemplate = b))
            })
        },
        options: m,
        name: p,
        version: "0.6"
    })
})(jQuery);
(function(c) {
    var p = "spiral",
        m = {
            series: {
                spiral: {
                    active: !1,
                    show: !1,
                    spiralSize: 0.8,
                    rotations: 3,
                    steps: 36,
                    delay: 50,
                    highlight: { opacity: 0.5 },
                    debug: { active: !1, createDocuTemplate: null }
                }
            }
        },
        x = { series: { nearBy: { distance: 6, findItem: null, findMode: "circle", drawEdit: null, drawHover: null } } },
        t = { grid: { show: !1 } },
        q = c.plot.JUMlib.data.createColors,
        n = c.plot.JUMlib.data.getColor;
    c.plot.plugins.push({
        init: function(a) {
            function b() {
                var a, b;
                a = c.plot.JUMExample.docuObjectToTemplate([
                        { name: "data", tree: j.data }, {
                            name: "options.series.spiral",
                            tree: m.series.spiral,
                            takeDefault: !0
                        }, { name: "options.series.spiral", tree: g.series.spiral }
                    ],
                    p);
                c.plot.JUMExample.extendDocuObject(a, p);
                b = c.plot.JUMExample.docuObjectToEdit(a, "");
                return{ data: a, form: b }
            }

            function h(a, b, c) {
                if (c.spiral.show) {
                    j = c;
                    a.getPlotOffset();
                    w = q(g, c.data.length);
                    r = Math.min(b.canvas.width, b.canvas.height) / 2 * g.series.spiral.spiralSize;
                    v = B = b.canvas.height / 2;
                    l = b;
                    for (a = u = 0; a < c.data.length; a++) u += c.data[a].data;
                    for (a = 0; a < c.data.length; a++)
                        z.push({
                            data: c.data[a].data,
                            size: 360 *
                            (c.data[a].data /
                                u)
                        })
                }
                s = A = 1;
                D = window.setInterval(f, g.series.spiral.delay)
            }

            function f() {
                var a, b, c, f;
                l.clearRect(0, 0, l.canvas.width, l.canvas.height);
                0 === g.series.spiral.rotations
                    ? (y = 0, c = g.series.spiral.steps)
                    : (y = 360 * A / g.series.spiral.steps, c = g.series.spiral.steps * g.series.spiral.rotations);
                for (var h = 0; h < z.length; h++)
                    b = (s - 1) * g.series.spiral.steps + A, a = b / c * r, b = b / c * z[h].size, f =
                            n({ ctx: l, serie: j, dataIndex: h, colors: w, radius: a, left: v, top: B }),
                        d(l, y, y + b, a, f), y += b;
                A++;
                A > g.series.spiral.steps &&
                (A = 1, s++, s > g.series.spiral.rotations &&
                (window.clearInterval(D), j.nearBy.findItem = e, j.nearBy.drawHover = k))
            }

            function d(a, b, c, d, e) {
                b = 2 * Math.PI * y / 360;
                c = 2 * Math.PI * c / 360;
                var f = v + Math.round(Math.cos(b) * d), g = B + Math.round(Math.sin(b) * d);
                a.strokeStyle = e;
                a.fillStyle = e;
                a.beginPath();
                a.moveTo(v, B);
                a.lineTo(f, g);
                a.arc(v, B, d, b, c);
                a.lineTo(v, B);
                a.closePath();
                a.fill()
            }

            function e(a, b, c) {
                a: {
                    var d = null, e;
                    for (e = y = 0; e < z.length; e++) {
                        l.save();
                        l.beginPath();
                        var f = 2 * Math.PI * y / 360,
                            g = 2 * Math.PI * (y + z[e].size) / 360,
                            h = v + Math.round(Math.cos(f) * r),
                            j = B +
                                Math.round(Math.sin(f) *
                                    r);
                        l.moveTo(v, B);
                        l.lineTo(h, j);
                        l.arc(v, B, r, f, g);
                        l.closePath();
                        if (l.isPointInPath(a, b)) {
                            d = [c, e];
                            l.restore();
                            a = d;
                            break a
                        }
                        y += z[e].size
                    }
                    a = d
                }
                return a
            }

            function k(a, b, c) {
                for (var e = y = 0; e < c; e++) y += z[e].size;
                d(a, y, y + z[c].size, r, "rgba(255,255,255," + b.spiral.highlight.opacity + ")")
            }

            var g = null, j = null, l, w, u, r, v, B, A, s, z = [], D, y;
            a.hooks.processOptions.push(function(a, d) {
                !0 === d.series.spiral.active &&
                (c.extend(!0, d, t), c.plot.JUMlib.data.extendEmpty(d, x), g = d, a.hooks.drawSeries
                    .push(h), !0 === g.series.spiral.debug.active &&
                (g.series.spiral.debug.createDocuTemplate = b))
            })
        },
        options: m,
        name: p,
        version: "0.3"
    })
})(jQuery);
(function(c) {
    var p = "heatmap",
        m = {
            series: {
                heatmap: {
                    active: !1,
                    show: !1,
                    backImage: null,
                    radiusIn: 10,
                    radiusOut: 20,
                    visible: !0,
                    width: 0,
                    height: 0,
                    max: !1,
                    gradient: { "0.45": "rgb(0,0,255)", "0.55": "rgb(0,255,255)", "0.65": "rgb(0,255,0)", "0.95": "yellow", 1: "rgb(255,0,0)" },
                    opacity: 180,
                    highlight: { opacity: 0.5 },
                    debug: { active: !1, createDocuTemplate: null }
                }
            }
        },
        x = { series: { nearBy: { distance: 6, findItemDefault: null, findMode: "circle", drawHover: null } } };
    c.plot.plugins.push({
        init: function(t) {
            function q(a, b) {
                !0 === b.heatmap.show &&
                (b.nearBy.findItemDefault =
                    b.nearBy.findItem, b.nearBy.findItem = h, (new Image).src = f.series.heatmap.backImage)
            }

            function n() {
                var a, b;
                a = c.plot.JUMExample.docuObjectToTemplate([
                        { name: "data", tree: g.data }, {
                            name: "options.series.heatmap",
                            tree: m.series.heatmap,
                            takeDefault: !0
                        },
                        { name: "options.series.heatmap", tree: f.series.heatmap },
                        { name: "options.series.editMode", tree: m.series.editMode, takeDefault: !0 },
                        { name: "options.series.editMode", tree: f.series.editMode },
                        { name: "options.series.nearBy", tree: m.series.nearBy, takeDefault: !0 }, {
                            name: "options.series.nearBy",
                            tree: f.series.nearBy
                        }
                    ],
                    p);
                c.plot.JUMExample.extendDocuObject(a, p);
                b = c.plot.JUMExample.docuObjectToEdit(a, "");
                return{ data: a, form: b }
            }

            function a(a, b) {
                var c;
                f.series.heatmap.backImage &&
                (c = f.series.heatmap.backImage, d = a.getPlotOffset(), b.save(), b.translate(d.left, d.top), b
                    .drawImage(c, 0, 0, a.width(), a.height()), b.restore())
            }

            function b(a, b, c) {
                !0 === f.series.heatmap.debug.active && (g = c);
                e = document.createElement("canvas");
                e.style.top = "0px";
                e.style.left = "0px";
                e.style.position = "absolute";
                e.height = b.canvas.height;
                e.width = b.canvas.width;
                k = e.getContext("2d");
                d = a.getPlotOffset();
                for (var h = c.data.length - 1; 0 <= h; h--) {
                    var r = c.data[h],
                        m = b,
                        n = k,
                        p = c.xaxis.p2c(r[0]),
                        s = c.yaxis.p2c(r[1]),
                        r = r[2],
                        q = c.heatmap.radiusOut,
                        t = n,
                        y = t.createRadialGradient(p, s, c.heatmap.radiusIn, p, s, q),
                        p = p - q,
                        s = s - q,
                        q = 2 * q;
                    y.addColorStop(0, "rgba(0,0,0," + (r ? r / c.heatmap.max : "0.1") + ")");
                    y.addColorStop(1, "rgba(0,0,0,0)");
                    t.fillStyle = y;
                    t.fillRect(p, s, q, q);
                    q = n;
                    r = p;
                    t = s;
                    n = d;
                    p = a.width();
                    s = a.heigth;
                    y = 2 * c.heatmap.radiusOut;
                    r + y > p && (r = p - y);
                    0 > r && (r = 0);
                    0 > t && (t = 0);
                    t +
                        y >
                        s &&
                        (t = s - y);
                    for (var p = q.getImageData(r, t, y, y).data,
                        s = p.length,
                        r = r + n.left,
                        t = t + n.top,
                        y = m.getImageData(r, t, y, y),
                        q = y.data,
                        x = f.series.heatmap.gradient,
                        F = f.series.heatmap.opacity,
                        C = 3;
                        C < s;
                        C += 4) {
                        var G = p[C];
                        if (n = 4 * G) q[C - 3] = x[n], q[C - 2] = x[n + 1], q[C - 1] = x[n + 2], q[C] = G < F ? G : F
                    }
                    y.data = q;
                    m.putImageData(y, r, t)
                }
            }

            function h(a, b, c, d) {
                var e = null;
                return e = d.nearBy.findItemDefault(a, b, c, d)
            }

            var f = null, d = "7", e = null, k = null, g = null;
            t.hooks.processOptions.push(function(d, e) {
                if (e.series.heatmap.active) {
                    c.plot.JUMlib.data.extendEmpty(e,
                        x);
                    f = e;
                    d.hooks.processRawData.push(q);
                    d.hooks.drawBackground.push(a);
                    d.hooks.drawSeries.push(b);
                    !0 === f.series.heatmap.debug.active && (f.series.heatmap.debug.createDocuTemplate = n);
                    var g = document.createElement("canvas");
                    g.width = "1";
                    g.height = "256";
                    var h = g.getContext("2d"),
                        k = h.createLinearGradient(0, 0, 1, 256),
                        m = f.series.heatmap.gradient,
                        p;
                    for (p in m) k.addColorStop(p, m[p]);
                    h.fillStyle = k;
                    h.fillRect(0, 0, 1, 256);
                    f.series.heatmap.gradient = h.getImageData(0, 0, 1, 256).data;
                    delete g;
                    delete k;
                    delete h
                }
            })
        },
        options: m,
        name: p,
        version: "0.3"
    })
})(jQuery);
(function(c) {
    var p = "animate",
        m = {
            animate: {
                active: !1,
                mode: "tile",
                tile: { x: 3, y: 3, mode: "lt" },
                pixastic: { maxValue: 1, mode: "blurfast" },
                stepDelay: 500,
                steps: 20,
                debug: { active: !1, createDocuTemplate: null }
            }
        };
    c.plot.plugins.push({
        init: function(x) {
            function t() {
                var b, d;
                b = c.plot.JUMExample.docuObjectToTemplate([
                        { name: "options.animate", tree: m.animate, takeDefault: !0 },
                        { name: "options.animate", tree: a.animate }
                    ],
                    p);
                c.plot.JUMExample.extendDocuObject(b, p);
                d = c.plot.JUMExample.docuObjectToEdit(b, "");
                return{ data: b, form: d }
            }

            function q(b,
                c) {
                h = c;
                a = b.getOptions();
                b.getPlotOffset()
            }

            function n() {
                var c;
                c = document.createElement("canvas").getContext("2d");
                c.canvas.width = h.canvas.width;
                c.canvas.height = h.canvas.height;
                c.putImageData(h.getImageData(0, 0, h.canvas.width, h.canvas.height), 0, 0);
                h.clearRect(0, 0, h.canvas.width, h.canvas.height);
                switch (a.animate.mode) {
                case "tile":
                    var d = a.animate.tile,
                        e = function() {
                            h.putImageData(c.getImageData(k * l, g * n, l, n), k * l, g * n);
                            switch (d.mode) {
                            case "lt":
                                k++ >= d.x && (k = 0, g++ >= d.y && window.clearInterval(b));
                                break;
                            case "tl":
                                g++ >=
                                    d.y &&
                                    (g = 0, k++ >= d.x && window.clearInterval(b));
                                break;
                            case "rb":
                                0 > k-- && (k = d.x - 1, 0 > g-- && window.clearInterval(b));
                                break;
                            case "br":
                                0 > g-- && (g = d.y - 1, 0 > k-- && window.clearInterval(b));
                                break;
                            case "random":
                                if (0 === j.length) window.clearInterval(b);
                                else {
                                    var a = parseInt(Math.random() * j.length);
                                    k = j[a][0];
                                    g = j[a][1];
                                    j.splice(a, 1)
                                }
                            }
                        },
                        k,
                        g,
                        j = [],
                        l = h.canvas.width / d.x,
                        n = h.canvas.height / d.y,
                        u = new Date,
                        r = a.animate.stepDelay;
                    switch (d.mode) {
                    case "lt":
                        g = k = 0;
                        break;
                    case "tl":
                        g = k = 0;
                        break;
                    case "rb":
                        k = d.x - 1;
                        g = d.y - 1;
                        break;
                    case "br":
                        k =
                            d.x - 1;
                        g = d.y - 1;
                        break;
                    case "random":
                        for (var m = 0; m < d.x; m++) for (var p = 0; p < d.y; p++) j.push([m, p]);
                        m = parseInt(Math.random() * j.length);
                        k = j[m][0];
                        g = j[m][1];
                        j.splice(m, 1)
                    }
                    e();
                    r -= new Date - u;
                    b = window.setInterval(e, r);
                    break;
                case "pixastic":
                    var e = function() {
                            var d;
                            t[x] = q;
                            if (0 === q)
                                h.putImageData(c.getImageData(0, 0, c.canvas.width, c.canvas.height), 0, 0), window
                                    .clearInterval(b);
                            else if (d = Pixastic.process(c.canvas, a.animate.pixastic.mode, t).getContext("2d"), h
                                .putImageData(d.getImageData(0, 0, d.canvas.width, d.canvas.height),
                                    0,
                                    0), q -= s, 0 < s && 0 >= q || 0 > s && 0 <= q) q = 0
                        },
                        q,
                        s,
                        t = {},
                        x,
                        u = new Date;
                    switch (a.animate.pixastic.mode) {
                    case "blurfast":
                        q = 2.5 * Math.abs(a.animate.pixastic.maxValue);
                        x = "amount";
                        t = { amount: 2.5 };
                        break;
                    case "lighten":
                        q = Math.min(1, Math.max(-1, a.animate.pixastic.maxValue));
                        x = "amount";
                        t = { amount: 1 };
                        break;
                    case "emboss":
                        q = 10 * Math.abs(a.animate.pixastic.maxValue);
                        x = "strength";
                        t = { greyLevel: 127, direction: "topleft", blend: !0 };
                        break;
                    case "mosaic":
                        q = parseInt(100 * Math.abs(a.animate.pixastic.maxValue));
                        x = "blockSize";
                        t = { blockSize: 100 };
                        break;
                    case "noise":
                        q = Math.abs(a.animate.pixastic.maxValue);
                        x = "strength";
                        t = { mono: !0, amount: 1, strength: 0.5 };
                        break;
                    default:
                        q = Math.min(1, Math.max(-1, a.animate.pixastic.maxValue))
                    }
                    s = q / a.animate.steps;
                    e();
                    b = window.setInterval(e, a.animate.stepDelay - (new Date - u));
                    break;
                default:
                    h.putImageData(c.getImageData(0, 0, c.canvas.width, c.canvas.height), 0, 0)
                }
            }

            var a, b, h;
            x.hooks.processOptions.push(function(b, c) {
                !0 === c.animate.active &&
                (b.hooks.draw.push(q), b.hooks.bindEvents.push(n), a = c, !0 === c.animate.debug.active &&
                (a.animate.debug.createDocuTemplate =
                    t))
            })
        },
        options: m,
        name: p,
        version: "0.2"
    })
})(jQuery);
(function(c) {
    var p = "video",
        m = {
            series: {
                video: {
                    active: !1,
                    show: !1,
                    stepAction: "simple",
                    stepCollection: {
                        simple: {
                            runStep: function(m, n) {
                                var a;
                                if (n.walkPad)
                                    return a = c.Deferred(), c(n.walkPad).append("<br>" + m.data[2]), window
                                        .setTimeout(function() { a.resolve() }, n.walkTime), a.promise();
                                alert(m.data[2])
                            },
                            walkPad: "#stepPad",
                            walkTime: 2E3
                        },
                        youtube: {
                            runStep: function(m, n) {
                                function a() {
                                    jQuery(n.videoPad).tubeplayer("destroy");
                                    b.resolve()
                                }

                                var b;
                                if (n.videoPad)
                                    return b = c.Deferred(), 3 < m.data.length
                                        ? "string" === typeof m.data[3] &&
                                        (jQuery.tubeplayer.defaults
                                                .afterReady = function() { jQuery(n.videoPad).tubeplayer("play") },
                                            jQuery(n.videoPad)
                                                .tubeplayer({
                                                    width: n.width,
                                                    height: n.height,
                                                    initialVideo: m.data[3],
                                                    onPlayerEnded: a,
                                                    onStop: a
                                                }), jQuery(n.videoPad).tubeplayer("play"), window
                                                .setTimeout(function() { a() }, n.maxDuration))
                                        : window.setTimeout(function() { b.resolve() }, n.noVideoDuration), b.promise();
                                alert("no videoPad defined")
                            },
                            videoPad: "#videoPad",
                            width: 400,
                            height: 300,
                            maxDuration: 2E4,
                            noVideoDuration: 2E3
                        }
                    },
                    debug: { active: !1, createDocuTemplate: null }
                }
            }
        },
        x = { grid: { show: !1 } },
        t = {};
    c.plot.plugins.push({
        init: function(q) {
            function n() {
                var a, b;
                a = c.plot.JUMExample.docuObjectToTemplate([
                        { name: "data", tree: g.data }, {
                            name: "options.series.video",
                            tree: m.series.video,
                            takeDefault: !0
                        },
                        { name: "options.series.video", tree: e.series.video }
                    ],
                    p);
                c.plot.JUMExample.extendDocuObject(a, p);
                b = c.plot.JUMExample.docuObjectToEdit(a, "");
                return{ data: a, form: b }
            }

            function a(a) {
                var b, c;
                if (!0 === e.series.video.active && !1 === j) {
                    d = a.getData();
                    for (b = 0; b < d.length; b++)
                        if (!0 === d[b].video.show) {
                            g = d[b].data;
                            w = Math.max(w, d[b].data.length);
                            d[b].dataOrg = f(d[b].data);
                            for (c = 0; c < d[b].data.length; c++) d[b].data[c] = null
                        }
                    a.setData(d);
                    j = !0
                }
            }

            function b(a) { !0 === e.series.video.active && (k = a, window.setTimeout(h, 0)) }

            function h() {
                var a, b = [], e, f;
                for (a = 0; a < d.length; a++)
                    !0 === d[a].video.show &&
                    (d[a].data[l] = d[a].dataOrg[l], k.setData(d), k
                            .draw(), f = d[a]
                            .video, e = { seriesIndex: a, dataIndex: l, data: d[a].data[l], serie: d[a] },
                        "string" === typeof f.stepAction
                            ? b.push(f.stepCollection[f.stepAction].runStep(e, f.stepCollection[f.stepAction]))
                            : "object" === typeof f.stepAction && b.push(f.stepAction.runStep(e, f.stepAction)));
                l++;
                l < w && c.when.apply(null, b).then(function() { h() })
            }

            function f(a) {
                if (null === a || "object" !== typeof a) return a;
                var b = new a.constructor, c;
                for (c in a) b[c] = f(a[c]);
                return b
            }

            var d = null, e = null, k = null, g = null, j = !1, l = 0, w = 0;
            q.hooks.processOptions.push(function(d, f) {
                f.series.video.active &&
                (e = f, c.extend(!0, f, x), c.plot.JUMlib.data.extendEmpty(f, t), e = f, d.hooks.draw.push(a), d.hooks
                    .bindEvents.push(b), !0 === e.series.video.debug.active &&
                (e.series.video.debug.createDocuTemplate =
                    n))
            })
        },
        options: m,
        name: p,
        version: "0.2"
    })
})(jQuery);
(function(c) {
    var p = "contour",
        m = {
            series: {
                contour: { active: !1, show: !1, ellipseStep: 0.1, debug: { active: !1, createDocuTemplate: null } }
            },
            grid: { overlay: { image: null, opacity: 0.2 } }
        },
        x = {},
        t = { series: { editMode: "xy", nearBy: { distance: 7, findItem: null, findMode: "circle", drawHover: null } } };
    c.plot.plugins.push({
        init: function(q) {
            function n(a, b) { !0 === b.contour.show && (b.nearBy.findItem = d, b.nearBy.drawHover = e) }

            function a() {
                var a, b;
                a = c.plot.JUMExample.docuObjectToTemplate([
                        { name: "data", tree: j.data }, {
                            name: "options.series.contour",
                            tree: m.series.contour,
                            takeDefault: !0
                        }, { name: "options.series.contour", tree: g.series.contour },
                        { name: "options.series.nearBy", tree: m.series.nearBy, takeDefault: !0 },
                        { name: "options.series.nearBy", tree: g.series.nearBy }
                    ],
                    p);
                c.plot.JUMExample.extendDocuObject(a, p);
                b = c.plot.JUMExample.docuObjectToEdit(a, "");
                return{ data: a, form: b }
            }

            function b(a, b, c) {
                if (c.contour.show) {
                    !0 === g.series.contour.debug.active && (j = c);
                    k = a.getPlotOffset();
                    for (a = 0; a < c.data.length; a++) f(b, c, a, c.color)
                }
            }

            function h(a) {
                var b = g.grid.overlay.image,
                    d = '<div style="position:absolute;width:' +
                        a.width() +
                        ";height:" +
                        a.height() +
                        ";top:" +
                        k.top +
                        ";left:" +
                        k.left +
                        ';">',
                    d = c(d);
                c(b).css("opacity", g.grid.overlay.opacity).width(a.width()).height(a.height());
                c(b).css("top", k.top).css("position", "absolute").css("left", k.left);
                c(b).appendTo(d);
                d.appendTo(a.getPlaceholder())
            }

            function f(a, b, c, d) {
                var e = b.data[c];
                c = parseInt(k.left + b.xaxis.p2c(e[0]), 0);
                var f = parseInt(k.top + b.yaxis.p2c(e[1]), 0),
                    g = parseInt(b.xaxis.scale * e[2], 0),
                    h = parseInt(b.yaxis.scale * e[3], 0),
                    e = e[4],
                    j,
                    m;
                b = b.contour.ellipseStep;
                g /= 2;
                m = h / 2;
                h = c + g * Math.cos(e);
                j = f + g * Math.sin(e);
                a.save();
                a.beginPath();
                a.fillStyle = d;
                a.moveTo(h, j);
                for (d = b; d < 2 * Math.PI; d += b)
                    h = c + g * Math.cos(d) * Math.cos(e) - m * Math.sin(d) * Math.sin(e), j =
                        f + g * Math.cos(d) * Math.sin(e) + m * Math.sin(d) * Math.cos(e), a.lineTo(h, j);
                a.closePath();
                a.fill();
                a.restore()
            }

            function d(a, b, c, d) {
                a = null;
                if (g.series.justEditing)
                    g.series.justEditing[1].seriesIndex === c &&
                    (d = null, a = d = [c, g.series.justEditing[1].dataIndex]);
                else {
                    if (d.contour.show) for (c = 0; c < d.data.length; c++);
                    a = null
                }
                return a
            }

            function e(a, b, c) {
                a.save();
                a.translate(-k.left, -k.top);
                f(a, b, c, "rgba(255,255,255," + b.bandwidth.highlight.opacity + ")");
                a.restore()
            }

            var k = null, g = null, j = null;
            q.hooks.processOptions.push(function(d, e) {
                e.series.contour.active &&
                (c.extend(!0, e, x), c.plot.JUMlib.data.extendEmpty(e, t), g = e, d.hooks.processRawData.push(n), d
                    .hooks.drawSeries.push(b), g.grid.overlay
                    .image &&
                    d.hooks.draw.push(h), !0 === g.series.contour.debug.active &&
                (g.series.contour.debug.createDocuTemplate = a))
            })
        },
        options: m,
        name: p,
        version: "0.1"
    })
})(jQuery);