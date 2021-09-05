using System;
using System.Drawing;
using System.Collections;

namespace Endogine.ColorEx
{
	/// <summary>
	/// Summary description for ColorFunctions.
	/// </summary>
	public class ColorFunctions
	{
		public ColorFunctions()
		{
		}

		public static ColorHsb RgbToHsb(Color c)
		{
			return new ColorHsb(c);
		}

		public static Color HsbToRgb(ColorHsb hsb)
		{
			return hsb.ColorRGBA;
		}

		/*
on RGBToHSV(R, G, B)
  R=float(R)
  G=float(G)
  B=float(B)
  minVal=float(min(R, G, B))
  V=float(max(R, G, B))
  
  Delta=V-minVal
  
  -- Calculate saturation: saturation is 0 if r, g and b are all 0
  if V=0.0 then
    S=0.0
  else
    S=Delta / V
  end if
  
  if S=0.0 then
    H=0.0    -- Achromatic: When s = 0, h is undefined but who cares
  else       -- Chromatic
    if R<=V then -- between yellow and magenta [degrees]
      H=60.0*(G-B)/Delta
    else
      if G<=V then -- between cyan and yellow
        H=120.0+60.0*(B-R)/Delta
      else
        if B<=V then -- between magenta and cyan
          H=240.0+60.0*(R-G)/Delta
          if H<0.0 then H=H+360.0
        end if
      end if
    end if
  end if
  -- return a list of values as an rgb object would not be sensible
  return [h, s, v]
end RGBtoHSV

on HSVtoRGB(h, s, v)
  h=float(h)
  s=float(s)
  v=float(v)
  if S=0.0 then   -- color is on black-and-white center line
    R=V           -- achromatic: shades of gray
    G=V           -- supposedly invalid for h=0 when s=0 but who cares
    B=V
  else -- chromatic color
    if H=360.0 then  -- 360 degrees same as 0 degrees
      hTemp=0.0
    else
      hTemp=H
    end if
    
    hTemp=hTemp/60.0   -- h is now in [0,6)
    i=bitOr(hTemp, 0)  -- largest integer <= h
    f=hTemp-i          -- fractional part of h
    
    p=V*(1.0-S)
    q=V*(1.0-(S*f))
    t=V*(1.0-(S*(1.0-f)))
    
    case i of
      0:
        R = V
        G = t
        B = p
      1:
        R = q
        G = V
        B = p
      2:
        R = p
        G = V
        B = t
      3:
        R = p
        G = q
        B = V
      4:
        R = t
        G = p
        B = V
      5:
        R = V
        G = p
        B = q
    end case
  end if
  return rgb(R*255, G*255, B*255)
end
*/
		public static Color InterpolateBetweenRGB(Color c1, Color c2, float position)
		{
			return Color.FromArgb(
				(int)(position*(c1.A-c2.A)+c2.A),
				(int)(position*(c1.R-c2.R)+c2.R),
				(int)(position*(c1.G-c2.G)+c2.G),
				(int)(position*(c1.B-c2.B)+c2.B));
		}

        public static Vector4 ColorToVector4(Color color)
        {
            return new Vector4((float)color.R / 255, (float)color.G / 255, (float)color.B / 255, (float)color.A / 255);
        }

        public static Color Vector4ToColor(Vector4 v)
        {
            return Color.FromArgb((int)(v.W * 255), (int)(v.X * 255), (int)(v.Y * 255), (int)(v.Z * 255));
        }
	}
}
